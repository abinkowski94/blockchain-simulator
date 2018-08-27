using System;
using BlockchainSimulator.Hub.BusinessLogic.Model;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public interface ISimulationStorage
    {
        Simulation GetSimulation(Guid scenarioId);
        void SaveChanges();
    }
}