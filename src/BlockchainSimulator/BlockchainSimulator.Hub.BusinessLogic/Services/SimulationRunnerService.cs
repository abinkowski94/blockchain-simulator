using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Queues;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationRunnerService : ISimulationRunnerService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly string _directoryPath;
        private readonly TimeSpan _nodeTimeout;
        private readonly string _pathToLibrary;
        private readonly object _padlock = new object();

        public SimulationRunnerService(IBackgroundTaskQueue queue, IHostingEnvironment environment)
        {
            _queue = queue;
            _directoryPath = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
            //TODO: Add timeout configuration
            _nodeTimeout = TimeSpan.FromSeconds(10);

            var pathToFolder = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            _pathToLibrary = $@"{pathToFolder}\{nameof(BlockchainSimulator)}.{nameof(Node)}.{nameof(Node.WebApi)}.dll";
        }

        public void RunSimulation(Simulation simulation, SimulationSettings settings)
        {
            lock (_padlock)
            {
                simulation.Status = SimulationStatuses.Pending;
                SpawnServers(simulation);
                PingServers(simulation);
                ConnectNodes(simulation);
            }
        }

        private void SpawnServers(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(cancellationToken => new Task(() =>
            {
                simulation.Status = SimulationStatuses.Preparing;
                var nodesToSpawn = simulation.ServerNodes.Where(n => n.NeedsSpawn).ToList();
                var parallelOptions = GetParallelOptions(cancellationToken);

                Parallel.ForEach(nodesToSpawn, parallelOptions, (node, state) =>
                {
                    if (parallelOptions.CancellationToken.IsCancellationRequested) return;
                    var pathToDirectory = $@"{_directoryPath}\nodes\{node.Id}";
                    if (!Directory.Exists(pathToDirectory))
                    {
                        Directory.CreateDirectory(pathToDirectory);
                    }

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
                });
            }, cancellationToken));
        }

        private void PingServers(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                var parallelOptions = GetParallelOptions(token);
                
                Parallel.ForEach(simulation.ServerNodes, parallelOptions, (node, state) =>
                {
                    if (parallelOptions.CancellationToken.IsCancellationRequested) return;
                    try
                    {
                        using (var httpClientHandler = new HttpClientHandler())
                        {
                            // Turns off SSL
                            httpClientHandler.ServerCertificateCustomValidationCallback = (msg, cert, ch, err) => true;
                            using (var httpClient = new HttpClient(httpClientHandler) {Timeout = _nodeTimeout})
                            {
                                var responseTask = httpClient.GetAsync($"{node.HttpAddress}/api/info", token);
                                responseTask.Wait(token);
                                node.IsConnected = responseTask.Result.IsSuccessStatusCode;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        node.IsConnected = false;
                    }
                });
            }, token));
        }

        private void ConnectNodes(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                var aliveNodes = simulation.ServerNodes.Where(n => n.IsConnected == true);
                var parallelOptions = GetParallelOptions(token);
                
                Parallel.ForEach(aliveNodes, parallelOptions, (node, state) =>
                {
                    if (parallelOptions.CancellationToken.IsCancellationRequested) return;
                    try
                    {
                        using (var httpClientHandler = new HttpClientHandler())
                        {
                            // Turns off SSL
                            httpClientHandler.ServerCertificateCustomValidationCallback = (msg, cert, ch, err) => true;
                            using (var httpClient = new HttpClient(httpClientHandler) {Timeout = _nodeTimeout})
                            {
                                var otherNodes = simulation.ServerNodes.Where(n => node.ConnectedTo.Contains(n.Id));
                                foreach (var otherNode in otherNodes)
                                {
                                    var body = JsonConvert.SerializeObject(otherNode);
                                    var content = new StringContent(body, Encoding.UTF8, "application/json");
                                    var task = httpClient.PutAsync($"{node.HttpAddress}/api/consensus", content, token);
                                    task.Wait(token);
                                }
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                });
            }, token));
        }

        private static ParallelOptions GetParallelOptions(CancellationToken cancellationToken)
        {
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = cancellationToken,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };
            return parallelOptions;
        }
    }
}