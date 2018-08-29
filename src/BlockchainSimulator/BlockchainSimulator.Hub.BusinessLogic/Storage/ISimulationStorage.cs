using BlockchainSimulator.Hub.BusinessLogic.Model;
using System;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public interface ISimulationStorage
    {
        Simulation GetSimulation(Guid scenarioId);

        void SaveChanges();
    }
}