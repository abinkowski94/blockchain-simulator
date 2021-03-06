﻿using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface IScenarioService
    {
        BaseResponse<Scenario> CreateScenario(Scenario scenario);

        BaseResponse<Scenario> DuplicateScenario(Guid scenarioId);

        BaseResponse<Scenario> GetScenario(Guid scenarioId);

        BaseResponse<List<Scenario>> GetScenarios();

        BaseResponse<Scenario> RemoveScenario(Guid scenarioId);

        BaseResponse<Scenario> RenameScenario(Guid scenarioId, string newName);
    }
}