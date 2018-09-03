using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public interface IScenarioStorage : IDisposable
    {
        Scenario AddScenario(Scenario scenario);

        Scenario GetScenario(Guid scenarioId);

        List<Scenario> GetScenarios();

        Scenario RemoveScenario(Guid scenarioId);

        void SaveChanges();
    }
}