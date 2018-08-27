using System;
using System.Collections.Generic;
using BlockchainSimulator.Hub.BusinessLogic.Model;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public interface IScenarioStorage
    {
        Scenario AddScenario(Scenario scenario);
        Scenario GetScenario(Guid scenarioId);
        List<Scenario> GetScenarios();
        Scenario RemoveScenario(Guid scenarioId);
        void SaveChanges();
    }
}