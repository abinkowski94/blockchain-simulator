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

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationRunnerService : ISimulationRunnerService
    {
        private readonly string _directoryPath;
        private readonly int _hostingRetryCount;
        private readonly TimeSpan _hostingTime;
        private readonly IHttpService _httpService;
        private readonly TimeSpan _nodeTimeout;
        private readonly object _padlock = new object();
        private readonly string _pathToLibrary;
        private readonly IBackgroundTaskQueue _queue;
        private readonly IStatisticService _statisticService;

        public SimulationRunnerService(IBackgroundTaskQueue queue, IHttpService httpService,
            IHostingEnvironment environment, IStatisticService statisticService)
        {
            _queue = queue;
            _httpService = httpService;
            _statisticService = statisticService;
            _directoryPath = environment.ContentRootPath ?? Directory.GetCurrentDirectory();

            //TODO: Add timeout configuration
            _nodeTimeout = TimeSpan.FromSeconds(10);
            _hostingTime = TimeSpan.FromSeconds(5);
            _hostingRetryCount = 5;

            var pathToFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _pathToLibrary = $@"{pathToFolder}\Node\BlockchainSimulator.Node.WebApi.dll";
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
                WaitForStatistics(simulation, settings);
                ClearNodes(simulation);
            }
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

        private void ClearNodes(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.ParallelForEach(node =>
                {
                    node.NodeThread?.Kill();
                    node.NodeThread?.Dispose();
                    node.NodeThread = null;
                }, token);

                var directoryPath = $@"{_directoryPath}\nodes";
                if (Directory.Exists(directoryPath))
                {
                    Directory.Delete(directoryPath, true);
                }

                simulation.Status = SimulationStatuses.ReadyToRun;
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

        private void PingServers(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.Where(n => settings.NodesAndTransactions.Keys.Contains(n.Id))
                    .ParallelForEach(node =>
                    {
                        try
                        {
                            var responseMessage = _httpService.Get($"{node.HttpAddress}/api/info", _nodeTimeout,
                                _hostingRetryCount, token);
                            node.IsConnected = responseMessage.IsSuccessStatusCode;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            node.IsConnected = false;
                        }
                    }, token);
            }, token));
        }

        private void SendTransactions(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                var randomGenerator = new Random();
                simulation.LastRunTime = DateTime.UtcNow;
                simulation.Status = SimulationStatuses.Running;
                var transactionsSent = settings.TransactionsSent;
                simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
                {
                    if (settings.NodesAndTransactions.TryGetValue(node.Id, out var number))
                    {
                        Enumerable.Range(0, (int) number).ForEach(i =>
                        {
                            if (HasSimulationTimeElapsed(simulation, settings))
                            {
                                return;
                            }

                            var body = JsonConvert.SerializeObject(new Transaction
                            {
                                Sender = Guid.NewGuid().ToString(),
                                Recipient = Guid.NewGuid().ToString(),
                                Amount = randomGenerator.Next(1, 1000),
                                Fee = (decimal) randomGenerator.NextDouble()
                            });
                            var content = new StringContent(body, Encoding.UTF8, "application/json");
                            var responseMessage = _httpService.Post($"{node.HttpAddress}/api/transactions", content,
                                _nodeTimeout, token);
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

        private void SpawnServers(Simulation simulation, SimulationSettings settings)
        {
            _queue.QueueBackgroundWorkItem(token =>
            {
                return new Task(() =>
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
                }, token);
            });
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
                        try
                        {
                            var response = _httpService.Get($"{node.HttpAddress}/api/statistic/mining-queue",
                                _nodeTimeout, token);
                            var contentTask = response.Content.ReadAsStringAsync();
                            contentTask.Wait(token);
                            var content = contentTask.Result;
                            var miningQueueStatus = JsonConvert.DeserializeObject<MiningQueueStatus>(content);

                            return miningQueueStatus.IsEmpty;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            return false;
                        }
                    });

                    wait = !HasSimulationTimeElapsed(simulation, settings, wait);

                    // The interval
                    Thread.Sleep(TimeSpan.FromSeconds(5));
                } while (wait);
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
                    try
                    {
                        var response = _httpService.Get($"{node.HttpAddress}/api/statistic", _nodeTimeout, token);
                        var contentTask = response.Content.ReadAsStringAsync();
                        contentTask.Wait(token);
                        var content = contentTask.Result;
                        var statistic = JsonConvert.DeserializeObject<SuccessResponse<Statistic>>(content);

                        statistics.Add(statistic.Result);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }, token);

                _statisticService.ExtractAndSaveStatistics(statistics.ToList(), settings,
                    simulation.ScenarioId.ToString());
            }, token));
        }
    }
}