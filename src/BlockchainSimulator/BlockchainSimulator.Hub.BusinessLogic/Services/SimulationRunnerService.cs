using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using BlockchainSimulator.Hub.BusinessLogic.Model.Transactions;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Models.WebClient;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationRunnerService : ISimulationRunnerService
    {
        private readonly TimeSpan _hostingTime;
        private readonly TimeSpan _nodeTimeout;
        private readonly TimeSpan _networkWaitInterval;
        private readonly int _hostingRetryCount;
        private readonly string _pathToLibrary;
        private readonly string _directoryPath;
        private readonly object _padlock = new object();

        private readonly IStatisticService _statisticService;
        private readonly IBackgroundTaskQueue _queue;
        private readonly IHttpService _httpService;

        public SimulationRunnerService(IStatisticService statisticService, IBackgroundTaskQueue queue,
            IHttpService httpService, IHostingEnvironment environment)
        {
            //TODO: Add times to global configuration
            _nodeTimeout = TimeSpan.FromSeconds(10);
            _hostingTime = TimeSpan.FromSeconds(5);
            _networkWaitInterval = TimeSpan.FromSeconds(5);
            _hostingRetryCount = 5;

            var pathToFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _pathToLibrary = $@"{pathToFolder}\Node\BlockchainSimulator.Node.WebApi.dll";
            _directoryPath = environment.ContentRootPath ?? Directory.GetCurrentDirectory();

            _statisticService = statisticService;
            _httpService = httpService;
            _queue = queue;
        }

        public void RunSimulation(Simulation simulation, SimulationSettings settings)
        {
            lock (_padlock)
            {
                simulation.Status = SimulationStatuses.Pending;

                SpawnServers(simulation, settings);
                PingServers(simulation, settings);
                ConnectNodes(simulation);
                SendTransactions(simulation, settings);
                WaitForNetwork(simulation, settings);
                StopAllJobs(simulation);
                WaitForStatistics(simulation, settings);
                ClearNodes(simulation);
            }
        }

        private void SpawnServers(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.Status = SimulationStatuses.Preparing;

                if (Directory.Exists($@"{_directoryPath}\nodes"))
                {
                    Directory.Delete($@"{_directoryPath}\nodes", true);
                }

                simulation.ServerNodes.Where(n => n.NeedsSpawn && settings.NodesAndTransactions.Keys.Contains(n.Id))
                    .ParallelForEach(node =>
                    {
                        var pathToDirectory = $@"{_directoryPath}\nodes\{node.Id}";
                        Directory.CreateDirectory(pathToDirectory);

                        node.NodeThread = Process.Start(new ProcessStartInfo
                        {
                            ArgumentList =
                            {
                                _pathToLibrary,
                                $"urls|-|{node.HttpAddress}",
                                $@"contentRoot|-|{pathToDirectory}",
                                $"Node:Id|-|{node.Id}",
                                $"Node:Type|-|{simulation.BlockchainConfiguration.Type}",
                                $"BlockchainConfiguration:Version|-|{simulation.BlockchainConfiguration.Version}",
                                $"BlockchainConfiguration:Target|-|{simulation.BlockchainConfiguration.Target}",
                                $"BlockchainConfiguration:BlockSize|-|{simulation.BlockchainConfiguration.BlockSize}"
                            },
                            FileName = "dotnet"
                        });
                    }, token);

                Thread.Sleep(_hostingTime);
            }, token));
        }

        private void PingServers(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.Where(n => settings.NodesAndTransactions.Keys.Contains(n.Id))
                    .ParallelForEach(node =>
                    {
                        var uri = $"{node.HttpAddress}/api/info";
                        var responseMessage = _httpService.Get(uri, _nodeTimeout, _hostingRetryCount, token);
                        node.IsConnected = responseMessage.IsSuccessStatusCode;
                    }, token);
            }, token));
        }

        private void ConnectNodes(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
                {
                    simulation.ServerNodes.Where(n => n.IsConnected == true && node.ConnectedTo.Contains(n.Id))
                        .ForEach(otherNode =>
                        {
                            var body = JsonConvert.SerializeObject(otherNode);
                            var content = new StringContent(body, Encoding.UTF8, "application/json");
                            _httpService.Put($"{node.HttpAddress}/api/consensus", content, _nodeTimeout, token);
                        });
                }, token);
            }, token));
        }

        private void SendTransactions(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.LastRunTime = DateTime.UtcNow;
                simulation.Status = SimulationStatuses.Running;

                var randomGenerator = new Random();
                var transactionsSent = settings.TransactionsSent;

                simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
                {
                    if (settings.NodesAndTransactions.TryGetValue(node.Id, out var number))
                    {
                        LinqExtensions.RepeatAction((int) number, () =>
                        {
                            if (HasSimulationTimeElapsed(simulation, settings))
                            {
                                return;
                            }

                            var transaction = new Transaction
                            {
                                Sender = Guid.NewGuid().ToString(),
                                Recipient = Guid.NewGuid().ToString(),
                                Amount = randomGenerator.Next(1, 1000),
                                Fee = (decimal) randomGenerator.NextDouble()
                            };

                            var uri = $"{node.HttpAddress}/api/transactions";
                            var content = new JsonContent(transaction);
                            var responseMessage = _httpService.Post(uri, content, _nodeTimeout, token);

                            if (responseMessage.IsSuccessStatusCode)
                            {
                                Interlocked.Increment(ref transactionsSent);
                            }
                        });
                    }
                }, token);

                settings.TransactionsSent = transactionsSent;
            }, token));
        }

        private void WaitForNetwork(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.Status = SimulationStatuses.WaitingForNetwork;
                bool wait;

                do
                {
                    wait = !simulation.ServerNodes.Where(n => n.IsConnected == true).All(node =>
                    {
                        var uri = $"{node.HttpAddress}/api/statistic/mining-queue";
                        var response = _httpService.Get(uri, _nodeTimeout, token);
                        return response.IsSuccessStatusCode && response.Content.ReadAs<MiningQueueStatus>().IsEmpty;
                    });

                    wait = !HasSimulationTimeElapsed(simulation, settings, wait);

                    // The interval
                    Thread.Sleep(_networkWaitInterval);
                } while (wait);
            }, token));
        }

        private void StopAllJobs(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
                {
                    var uri = $"{node.HttpAddress}/api/info";
                    var response = _httpService.Post(uri, new JsonContent(null), _nodeTimeout, token);
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Could not stop jobs for node id:{node.Id}");
                    }
                }, token);
            }, token));
        }

        private void WaitForStatistics(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.Status = SimulationStatuses.WaitingForStatistics;
                var statistics = new ConcurrentBag<Statistic>();

                simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
                {
                    var response = _httpService.Get($"{node.HttpAddress}/api/statistic", _nodeTimeout, token);
                    if (response.IsSuccessStatusCode)
                    {
                        var statistic = response.Content.ReadAs<SuccessResponse<Statistic>>();
                        statistics.Add(statistic.Result);
                    }
                }, token);

                var scenarioId = simulation.ScenarioId.ToString();
                _statisticService.ExtractAndSaveStatistics(statistics.ToList(), settings, scenarioId);
            }, token));
        }

        private void ClearNodes(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.ParallelForEach(node =>
                {
                    try
                    {
                        node.NodeThread?.Dispose();
                        node.NodeThread?.Kill();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        node.NodeThread = null;
                    }
                }, token);

                var directoryPath = $@"{_directoryPath}\nodes";
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }

                simulation.Status = SimulationStatuses.ReadyToRun;
            }, token));
        }

        private static bool HasSimulationTimeElapsed(Simulation simulation, SimulationSettings settings,
            bool wait = true)
        {
            if (wait && settings.ForceEndAfter.HasValue && simulation.LastRunTime.HasValue)
            {
                var timeDifference = DateTime.UtcNow - simulation.LastRunTime.Value;
                if (timeDifference > settings.ForceEndAfter)
                {
                    wait = false;
                }
            }

            return !wait;
        }
    }
}