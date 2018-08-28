using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Queues;
using Microsoft.AspNetCore.Hosting;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationRunnerService : ISimulationRunnerService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly string _directoryPath;
        private readonly object _padlock = new object();
        private readonly string _pathToLibrary;

        public SimulationRunnerService(IBackgroundTaskQueue queue, IHostingEnvironment environment)
        {
            _queue = queue;
            _directoryPath = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
            _pathToLibrary =
                $@"{Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)}\{nameof(BlockchainSimulator)}.{nameof(Node)}.{nameof(Node.WebApi)}.dll";
        }

        public void RunSimulation(Simulation simulation, SimulationSettings settings)
        {
            lock (_padlock)
            {
                simulation.Status = SimulationStatuses.Pending;
                PrepareThreads(simulation);
            }
        }

        private void PrepareThreads(Simulation simulation)
        {
            _queue.QueueBackgroundWorkItem(t => Task.Run(() =>
            {
                simulation.Status = SimulationStatuses.Preparing;
                simulation.ServerNodes.Where(n => n.NeedsSpawn).ToList().ForEach(n =>
                {
                    var pathToDirectory = $@"{_directoryPath}\nodes\{n.Id}";
                    if (!Directory.Exists(pathToDirectory))
                    {
                        Directory.CreateDirectory(pathToDirectory);
                    }

                    n.NodeThread = Process.Start(new ProcessStartInfo
                    {
                        ArgumentList =
                        {
                            _pathToLibrary,
                            $"urls|-|{n.HttpAddress}",
                            $@"contentRoot|-|{pathToDirectory}",
                            $"Node:Id|-|{n.Id}",
                            $"Node:Type|-|{simulation.BlockchainConfiguration.Type}",
                            $"BlockchainConfiguration:Version|-|{simulation.BlockchainConfiguration.Version}",
                            $"BlockchainConfiguration:Target|-|{simulation.BlockchainConfiguration.Target}",
                            $"BlockchainConfiguration:BlockSize|-|{simulation.BlockchainConfiguration.BlockSize}"
                        },
                        FileName = "dotnet"
                    });
                });
            }, t));
        }
    }
}