using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Queues;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationRunnerService : ISimulationRunnerService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IHttpService _httpService;
        private readonly string _directoryPath;
        private readonly TimeSpan _nodeTimeout;
        private readonly string _pathToLibrary;
        private readonly object _padlock = new object();

        public SimulationRunnerService(IBackgroundTaskQueue queue, IHttpService httpService,
            IHostingEnvironment environment)
        {
            _queue = queue;
            _httpService = httpService;
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
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.Status = SimulationStatuses.Preparing;
                simulation.ServerNodes.Where(n => n.NeedsSpawn).ParallelForEach(node =>
                {
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
                }, token);
            }, token));
        }

        private void PingServers(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.ParallelForEach(node =>
                {
                    try
                    {
                        var responseMessage = _httpService.Get($"{node.HttpAddress}/api/info", _nodeTimeout, token);
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

        private void ConnectNodes(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(token => new Task(() =>
            {
                simulation.ServerNodes.Where(n => n.IsConnected == true).ParallelForEach(node =>
                {
                    simulation.ServerNodes.Where(n => node.ConnectedTo.Contains(n.Id)).ForEach(otherNode =>
                    {
                        var body = JsonConvert.SerializeObject(otherNode);
                        var content = new StringContent(body, Encoding.UTF8, "application/json");
                        _httpService.Put($"{node.HttpAddress}/api/consensus", content, _nodeTimeout, token);
                    });
                }, token);
            }, token));
        }
    }
}