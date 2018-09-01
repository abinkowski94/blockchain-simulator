using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface ISimulationRunnerService
    {
        void RunSimulation(Simulation simulation, SimulationSettings settings);
    }
}