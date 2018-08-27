using BlockchainSimulator.Hub.BusinessLogic.Storage;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class SimulationService : ISimulationService
    {
        private readonly ISimulationStorage _simulationStorage;

        public SimulationService(ISimulationStorage simulationStorage)
        {
            _simulationStorage = simulationStorage;
        }
    }
}