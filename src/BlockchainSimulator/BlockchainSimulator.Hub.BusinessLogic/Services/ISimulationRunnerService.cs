using BlockchainSimulator.Hub.BusinessLogic.Model;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface ISimulationRunnerService
    {
        void RunSimulation(Simulation simulation, SimulationSettings settings);
    }
}