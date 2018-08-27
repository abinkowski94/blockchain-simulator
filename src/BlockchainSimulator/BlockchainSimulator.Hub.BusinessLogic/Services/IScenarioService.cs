using System;
using System.Collections.Generic;
using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface IScenarioService
    {
        BaseResponse<Scenario> CreateScenario(Scenario scenario);
        BaseResponse<Scenario> GetScenario(Guid scenarioId);
        BaseResponse<List<Scenario>> GetScenarios();
        BaseResponse<Scenario> RenameScenario(Guid scenarioId, string newName);
        BaseResponse<Scenario> RemoveScenario(Guid scenarioId);
        BaseResponse<Scenario> DuplicateScenario(Guid scenarioId);
    }
}