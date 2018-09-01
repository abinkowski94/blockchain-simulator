using BlockchainSimulator.Hub.BusinessLogic.Model;
using System;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public interface ISimulationStorage
    {
        Simulation GetSimulation(Guid scenarioId);

        void SaveChanges();
    }
}