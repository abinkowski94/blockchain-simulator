using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public class ScenarioService : IScenarioService
    {
        private readonly IScenarioStorage _scenarioStorage;

        public ScenarioService(IScenarioStorage scenarioStorage)
        {
            _scenarioStorage = scenarioStorage;
        }

        public BaseResponse<Scenario> CreateScenario(Scenario scenario)
        {
            var validationErrors = ValidateScenario(scenario);
            if (validationErrors.Any())
            {
                return new ErrorResponse<Scenario>("The scenario could not be added", scenario, validationErrors);
            }

            scenario.Id = Guid.NewGuid();
            scenario.CreateDate = DateTime.UtcNow;
            scenario.Simulation = new Simulation
            {
                ScenarioId = scenario.Id,
                BlockchainConfiguration = new BlockchainConfiguration(),
                ServerNodes = new List<ServerNode>(),
                Status = SimulationStatuses.ReadyToRun,
                LastRunTime = null
            };

            _scenarioStorage.AddScenario(scenario);
            _scenarioStorage.SaveChanges();

            return new SuccessResponse<Scenario>("The scenario has been created successfully!", scenario);
        }

        private static string[] ValidateScenario(Scenario scenario)
        {
            var errors = new List<string>();
            if (scenario == null)
            {
                errors.Add("The scenario cannot be null!");
                return errors.ToArray();
            }
            if (scenario.Name == null)
            {
                errors.Add("The scenario name cannot be null!");
            }

            return errors.ToArray();
        }
    }
}