using BlockchainSimulator.Hub.BusinessLogic.Model.Consensus;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;
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

        public BaseResponse<Scenario> DuplicateScenario(Guid scenarioId)
        {
            var scenario = _scenarioStorage.GetScenario(scenarioId);
            if (scenario == null)
            {
                return new ErrorResponse<Scenario>($"Could not find scenario with id: {scenarioId}!", null);
            }

            var duplicate = DuplicateScenario(scenario);
            _scenarioStorage.AddScenario(duplicate);
            _scenarioStorage.SaveChanges();

            return new SuccessResponse<Scenario>("The scenario has been duplicated!", duplicate);
        }

        public BaseResponse<Scenario> GetScenario(Guid scenarioId)
        {
            var scenario = _scenarioStorage.GetScenario(scenarioId);
            if (scenario == null)
            {
                return new ErrorResponse<Scenario>($"Could not find scenario with id: {scenarioId}!", null);
            }

            return new SuccessResponse<Scenario>("The scenario has been found!", scenario);
        }

        public BaseResponse<List<Scenario>> GetScenarios()
        {
            return new SuccessResponse<List<Scenario>>("The scenarios!", _scenarioStorage.GetScenarios());
        }

        public BaseResponse<Scenario> RemoveScenario(Guid scenarioId)
        {
            var scenario = _scenarioStorage.RemoveScenario(scenarioId);
            if (scenario == null)
            {
                return new ErrorResponse<Scenario>($"Could not find scenario with id: {scenarioId}!", null);
            }

            _scenarioStorage.SaveChanges();

            return new SuccessResponse<Scenario>("The scenario has been deleted!", scenario);
        }

        public BaseResponse<Scenario> RenameScenario(Guid scenarioId, string newName)
        {
            var scenario = _scenarioStorage.GetScenario(scenarioId);
            if (scenario == null)
            {
                return new ErrorResponse<Scenario>($"Could not find scenario with id: {scenarioId}!", null);
            }

            scenario.Name = newName;
            scenario.ModificationDate = DateTime.UtcNow;
            _scenarioStorage.SaveChanges();

            return new SuccessResponse<Scenario>("The scenario has been renamed!", scenario);
        }

        private static Scenario DuplicateScenario(Scenario scenario)
        {
            var newId = Guid.NewGuid();

            return new Scenario
            {
                Id = newId,
                Name = $"{scenario.Name} - Copy",
                CreateDate = DateTime.UtcNow,
                ModificationDate = null,
                Simulation = new Simulation
                {
                    ScenarioId = newId,
                    LastRunTime = null,
                    Status = SimulationStatuses.ReadyToRun,
                    BlockchainConfiguration = new BlockchainConfiguration
                    {
                        BlockSize = scenario.Simulation.BlockchainConfiguration.BlockSize,
                        Target = scenario.Simulation.BlockchainConfiguration.Target,
                        Version = scenario.Simulation.BlockchainConfiguration.Version,
                        Type = scenario.Simulation.BlockchainConfiguration.Type
                    },
                    ServerNodes = new List<ServerNode>(scenario.Simulation.ServerNodes.Select(n => new ServerNode
                    {
                        Id = n.Id,
                        Delay = n.Delay,
                        HttpAddress = n.HttpAddress,
                        IsConnected = n.IsConnected,
                        NeedsSpawn = n.NeedsSpawn,
                        ConnectedTo = new List<string>(n.ConnectedTo)
                    }))
                }
            };
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