using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Common.Models.WebClient;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using BlockchainSimulator.Hub.BusinessLogic.Model.Transactions;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationRunnerService : ISimulationRunnerService
    {
        private readonly TimeSpan _hostingTime;
        private readonly TimeSpan _nodeTimeout;
        private readonly TimeSpan _networkWaitInterval;
        private readonly int _hostingRetryCount;
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
            _hostingTime = TimeSpan.FromSeconds(10);
            _networkWaitInterval = TimeSpan.FromSeconds(5);
            _hostingRetryCount = 5;

            _directoryPath = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
            _statisticService = statisticService;
            _httpService = httpService;
            _queue = queue;
        }

        public void RunSimulation(Simulation simulation, SimulationSettings settings)
        {
            lock (_padlock)
            {
                // Change status to pending
                simulation.Status = SimulationStatuses.Pending;

                // Queue simulation
                _queue.QueueBackgroundWorkItem(token => new Task(() =>
                {
                    try
                    {
                        SpawnServers(simulation, settings, token);
                        PingServers(simulation, settings, token);
                        ConnectNodes(simulation, token);
                        SendTransactions(simulation, settings, token);
                        WaitForNetwork(simulation, settings, token);
                        StopAllJobs(simulation, token);
                        WaitForStatistics(simulation, settings, token);
                        ClearNodes(simulation, token);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    finally
                    {
                        simulation.Status = SimulationStatuses.ReadyToRun;
                    }
                }, token));
            }
        }

        private void SpawnServers(Simulation simulation, SimulationSettings settings, CancellationToken token)
        {
            // Set status
            simulation.Status = SimulationStatuses.Preparing;

            // Create temporary directory for noes storage
            if (Directory.Exists($@"{_directoryPath}\nodes"))
            {
                Directory.Delete($@"{_directoryPath}\nodes", true);
            }

            // Find the external dynamic link library for nodes to spawn
            var pathToFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var pathToLibrary = $@"{pathToFolder}\Node\BlockchainSimulator.Node.WebApi.dll";
            if (!File.Exists(pathToLibrary))
            {
                throw new SystemException(
                    "Could not find the library \"BlockchainSimulator.Node.WebApi.dll\" in order to spawn hubs!");
            }

            // Spawn nodes
            simulation.ServerNodes.Where(n => n.NeedsSpawn && settings.NodesAndTransactions.Keys.Contains(n.Id))
                .ParallelForEach(node =>
                {
                    var pathToDirectory = $@"{_directoryPath}\nodes\{node.Id}";
                    Directory.CreateDirectory(pathToDirectory);

                    node.NodeThread = Process.Start(new ProcessStartInfo
                    {
                        ArgumentList =
                        {
                            pathToLibrary,
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

            // Wait for processes to start
            Thread.Sleep(_hostingTime);
        }

        private void PingServers(Simulation simulation, SimulationSettings settings, CancellationToken token)
        {
            // Ping each node by using info endpoint
            simulation.ServerNodes.Where(n => settings.NodesAndTransactions.Keys.Contains(n.Id)).ParallelForEach(node =>
            {
                var uri = $"{node.HttpAddress}/api/info";
                var responseMessage = _httpService.Get(uri, _nodeTimeout, _hostingRetryCount, token);
                if ((node.IsConnected = responseMessage.IsSuccessStatusCode) != true)
                {
                    Console.WriteLine($"Could not ping server node: {node.Id}");
                }
            }, token);
        }

        private void ConnectNodes(Simulation simulation, CancellationToken token)
        {
            // Connect nodes that are connected
            simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
            {
                simulation.ServerNodes.Where(n => n.IsConnected == true && node.ConnectedTo.Contains(n.Id))
                    .ForEach(otherNode =>
                    {
                        var uri = $"{node.HttpAddress}/api/consensus";
                        var content = new JsonContent(otherNode);
                        var response = _httpService.Put(uri, content, _nodeTimeout, token);
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Could not connect node: {node.Id} with: {otherNode.Id}");
                        }
                    });
            }, token);
        }

        private void SendTransactions(Simulation simulation, SimulationSettings settings, CancellationToken token)
        {
            // Create random generator and referential value for number of transactions sent
            var randomGenerator = new Random();
            var transactionsSent = settings.TransactionsSent;

            // Change simulation status and run time
            simulation.Status = SimulationStatuses.Running;
            simulation.LastRunTime = DateTime.UtcNow;

            // Send given number of transactions to each node
            simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
            {
                if (settings.NodesAndTransactions.TryGetValue(node.Id, out var number))
                {
                    var uri = $"{node.HttpAddress}/api/transactions";
                    LinqExtensions.RepeatAction(number, () =>
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

                        var content = new JsonContent(transaction);
                        var responseMessage = _httpService.Post(uri, content, _nodeTimeout, token);

                        if (responseMessage.IsSuccessStatusCode)
                        {
                            // Interlocked is used because of parallel foreach
                            Interlocked.Increment(ref transactionsSent);
                        }
                    });
                }
            }, token);

            // Set the number of sent transactions
            settings.TransactionsSent = transactionsSent;
        }

        private void WaitForNetwork(Simulation simulation, SimulationSettings settings, CancellationToken token)
        {
            // Change status
            simulation.Status = SimulationStatuses.WaitingForNetwork;

            // TODO: To be simplified (Use signalR)
            bool wait;
            do
            {
                wait = !simulation.ServerNodes.Where(n => n.IsConnected == true).All(node =>
                {
                    var uri = $"{node.HttpAddress}/api/statistic/mining-queue";
                    var response = _httpService.Get(uri, _nodeTimeout, token);
                    return response.IsSuccessStatusCode && response.Content.ReadAs<MiningQueueStatus>().IsEmpty;
                });

                if (wait)
                {
                    wait = !HasSimulationTimeElapsed(simulation, settings);
                }

                // The interval
                Thread.Sleep(_networkWaitInterval);
            } while (wait);
        }

        private void StopAllJobs(Simulation simulation, CancellationToken token)
        {
            // Stopping all jobs in connected nodes
            simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
            {
                var uri = $"{node.HttpAddress}/api/info";
                var response = _httpService.Post(uri, new JsonContent(null), _nodeTimeout, token);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Could not stop jobs for node id:{node.Id}");
                }
            }, token);
        }

        private void WaitForStatistics(Simulation simulation, SimulationSettings settings, CancellationToken token)
        {
            // Change status
            simulation.Status = SimulationStatuses.WaitingForStatistics;

            // Get statistics in parallel mode
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

            // Extract statistics and save them
            var scenarioId = simulation.ScenarioId.ToString();
            _statisticService.ExtractAndSaveStatistics(statistics.ToList(), settings, scenarioId);
        }

        private void ClearNodes(Simulation simulation, CancellationToken token)
        {
            // Kill all nodes processes
            simulation.ServerNodes.ParallelForEach(node =>
            {
                try
                {
                    if (node.NodeThread?.CloseMainWindow() != true)
                    {
                        node.NodeThread?.Kill();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                finally
                {
                    node.NodeThread?.Dispose();
                    node.NodeThread = null;
                }
            }, token);

            // Wait for processes to end
            Thread.Sleep(_hostingTime);

            // Remove temporary directory
            var directoryPath = $@"{_directoryPath}\nodes";
            if (Directory.Exists(directoryPath))
            {
                Directory.Delete(directoryPath, true);
            }

            // Change status
            simulation.Status = SimulationStatuses.ReadyToRun;
        }

        private static bool HasSimulationTimeElapsed(Simulation simulation, SimulationSettings settings)
        {
            if (!settings.ForceEndAfter.HasValue || !simulation.LastRunTime.HasValue)
            {
                return false;
            }

            var timeDifference = DateTime.UtcNow - simulation.LastRunTime.Value;
            return timeDifference > settings.ForceEndAfter;
        }
    }
}