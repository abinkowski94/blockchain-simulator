using BlockchainSimulator.Hub.BusinessLogic.Model;
using System;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public class SimulationStorage : ISimulationStorage
    {
        private readonly IScenarioStorage _scenarioStorage;

        public SimulationStorage(IScenarioStorage scenarioStorage)
        {
            _scenarioStorage = scenarioStorage;
        }

        public Simulation GetSimulation(Guid scenarioId)
        {
            return _scenarioStorage.GetScenario(scenarioId)?.Simulation;
        }

        public void SaveChanges()
        {
            _scenarioStorage.SaveChanges();
        }
    }
}