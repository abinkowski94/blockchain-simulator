using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Hubs;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Common.Models.WebClient;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using BlockchainSimulator.Hub.BusinessLogic.Model.Transactions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        private readonly int _hostingRetryCount;
        private readonly string _directoryPath;
        private readonly object _padlock = new object();

        private readonly IStatisticService _statisticService;
        private readonly IBackgroundQueue _queue;
        private readonly IHttpService _httpService;

        public SimulationRunnerService(IStatisticService statisticService, IBackgroundQueue queue,
            IHttpService httpService, IHostingEnvironment environment)
        {
            //TODO: Add times to global configuration
            _nodeTimeout = TimeSpan.FromSeconds(10);
            _hostingTime = TimeSpan.FromSeconds(10);
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
                _queue.Enqueue(token => new Task(() =>
                {
                    try
                    {
                        SpawnServers(simulation, settings, token);
                        PingAndConnectWithServers(simulation, settings, token);
                        SetConfigurations(simulation, token);
                        ConnectNodes(simulation, token);
                        SendTransactions(simulation, settings, token);
                        WaitForNetwork(simulation, settings);
                        ForceMining(simulation, token);
                        WaitForNetwork(simulation, settings);
                        StopAllJobs(simulation, token);
                        WaitForStatistics(simulation, settings, token);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                    finally
                    {
                        ClearNodes(simulation, token);
                        simulation.Status = SimulationStatuses.ReadyToRun;
                    }
                }, token));
            }
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

        private void PingAndConnectWithServers(Simulation simulation, SimulationSettings settings,
            CancellationToken token)
        {
            // Ping each node by using info endpoint and connect in order to update status
            simulation.ServerNodes.Where(n => settings.NodesAndTransactions.Keys.Contains(n.Id)).ParallelForEach(node =>
            {
                var uri = $"{node.HttpAddress}/api/info";
                var responseMessage = _httpService.Get(uri, _nodeTimeout, _hostingRetryCount, token);
                if ((node.IsConnected = responseMessage.IsSuccessStatusCode) != true)
                {
                    Console.WriteLine($"Could not ping server node: {node.Id}");
                }
                else if (responseMessage.IsSuccessStatusCode)
                {
                    var url = $"{node.HttpAddress}/simulationHub";
                    node.HubConnection = new HubConnectionBuilder().WithUrl(url).Build();

                    // Reconnect when connection is closing
                    node.HubConnection.Closed += async error =>
                    {
                        await Task.Delay(new Random().Next(0, 5) * 1000, token);
                        await node.HubConnection.StartAsync(token);
                    };

                    // Register action: change of working status
                    const string methodName = nameof(ISiumlationClient.ChangeWorkingStatus);
                    node.HubConnection.On<bool>(methodName, isWorking => { node.IsWorking = isWorking; });

                    // Start the connection
                    node.HubConnection.StartAsync(token).Wait(token);
                }
            }, token);
        }

        private void SetConfigurations(Simulation simulation, CancellationToken token)
        {
            simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
            {
                var content = new JsonContent(new BlockchainNodeConfiguration
                {
                    BlockSize = Convert.ToInt32(simulation.BlockchainConfiguration.BlockSize),
                    NodeId = node.Id,
                    NodeType = simulation.BlockchainConfiguration.Type,
                    Version = simulation.BlockchainConfiguration.Version,
                    Target = simulation.BlockchainConfiguration.Target
                });
                var response = _httpService.Post($"{node.HttpAddress}/api/info/config", content, _nodeTimeout, token);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Could not set configuration for with id: {node.Id}");
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
                    var transactionsToSend = new List<Transaction>();
                    LinqExtensions.RepeatAction(number, () =>
                    {
                        if (!HasSimulationTimeElapsed(simulation, settings))
                        {
                            transactionsToSend.Add(new Transaction
                            {
                                Sender = Guid.NewGuid().ToString(),
                                Recipient = Guid.NewGuid().ToString(),
                                Amount = randomGenerator.Next(1, 1000),
                                Fee = (decimal) randomGenerator.NextDouble()
                            });
                        }
                    });

                    // Send transactions in one request together or one by one
                    if (settings.SendTransactionsTogether)
                    {
                        var uri = $"{node.HttpAddress}/api/transactions/multiple";
                        var content = new JsonContent(transactionsToSend);
                        var responseMessage = _httpService.Post(uri, content, _nodeTimeout, token);
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            // Interlocked is used because of parallel for-each
                            Interlocked.Add(ref transactionsSent, transactionsToSend.Count);
                        }
                    }
                    else
                    {
                        transactionsToSend.ForEach(t =>
                        {
                            if (!HasSimulationTimeElapsed(simulation, settings))
                            {
                                var uri = $"{node.HttpAddress}/api/transactions";
                                var content = new JsonContent(t);
                                var responseMessage = _httpService.Post(uri, content, _nodeTimeout, token);
                                if (responseMessage.IsSuccessStatusCode)
                                {
                                    // Interlocked is used because of parallel for-each
                                    Interlocked.Increment(ref transactionsSent);
                                }
                            }
                        });
                    }
                }
            }, token);

            // Set the number of sent transactions
            settings.TransactionsSent = transactionsSent;
        }

        private void WaitForNetwork(Simulation simulation, SimulationSettings settings)
        {
            do
            {
                // Change status and wait
                simulation.Status = SimulationStatuses.WaitingForNetwork;
                SpinWait.SpinUntil(() =>
                {
                    var wait = !simulation.ServerNodes.Where(n => n.IsConnected == true).Any(n => n.IsWorking);
                    if (!wait)
                    {
                        wait = HasSimulationTimeElapsed(simulation, settings);
                    }

                    return wait;
                });

                // Safe wait for network hiccups
                Thread.Sleep(TimeSpan.FromSeconds(10));

                // If its still working than wait
            } while (simulation.ServerNodes.Where(n => n.IsConnected == true).Any(n => n.IsWorking));
        }

        private void ForceMining(Simulation simulation, CancellationToken token)
        {
            // Forces mining for all nodes
            simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
            {
                var uri = $"{node.HttpAddress}/api/transactions/force-mining";
                var response = _httpService.Post(uri, new JsonContent(null), _nodeTimeout, token);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Could not force mining for: {node.Id}");
                }
            }, token);
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
                    Console.WriteLine($"Could not stop jobs for node id: {node.Id}");
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
            // Kill all processes of nodes
            simulation.ServerNodes.ParallelForEach(node =>
            {
                try
                {
                    if (node.IsConnected == true)
                    {
                        var response = _httpService.Post($"{node.HttpAddress}/api/info/clear", null, _nodeTimeout,
                            token);
                        if (!response.IsSuccessStatusCode)
                        {
                            Console.WriteLine($"Could not clear the node with id: {node.Id}");
                        }
                    }

                    node.HubConnection?.DisposeAsync();
                    if (node.NodeThread?.CloseMainWindow() != true)
                    {
                        node.NodeThread?.Kill();
                    }
                }
                finally
                {
                    node.NodeThread?.Dispose();
                    node.NodeThread = null;
                    node.HubConnection = null;
                    node.IsWorking = false;
                    node.IsConnected = null;
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
    }
}