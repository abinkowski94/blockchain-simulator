using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.DataAccess.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public class ScenarioStorage : IScenarioStorage
    {
        private readonly IFileRepository _fileRepository;
        private readonly object _padlock = new object();
        private readonly ConcurrentDictionary<Guid, Scenario> _scenarios;
        private readonly string _simulationFile;

        public ScenarioStorage(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _scenarios = new ConcurrentDictionary<Guid, Scenario>();
            _simulationFile = "simulations.json";
            PreloadScenarios();
        }

        public Scenario AddScenario(Scenario scenario)
        {
            _scenarios.TryAdd(scenario.Id, scenario);
            return scenario;
        }

        public void Dispose()
        {
            GetScenarios().SelectMany(s => s.Simulation.ServerNodes).Where(n => n.NodeThread != null)
                .ForEach(n =>
                {
                    n.NodeThread.Kill();
                    n.NodeThread.Dispose();
                    n.NodeThread = null;
                });
        }

        public Scenario GetScenario(Guid scenarioId)
        {
            _scenarios.TryGetValue(scenarioId, out var result);
            return result;
        }

        public List<Scenario> GetScenarios()
        {
            return _scenarios.Select(kv => kv.Value).ToList();
        }

        public Scenario RemoveScenario(Guid scenarioId)
        {
            _scenarios.TryRemove(scenarioId, out var result);
            return result;
        }

        public void SaveChanges()
        {
            lock (_padlock)
            {
                var data = JsonConvert.SerializeObject(_scenarios.Select(kv => kv.Value).ToList());
                _fileRepository.SaveFile(data, _simulationFile);
            }
        }

        private void PreloadScenarios()
        {
            var data = _fileRepository.GetFile(_simulationFile);
            if (data != null)
            {
                var scenariosList = JsonConvert.DeserializeObject<List<Scenario>>(data);
                scenariosList.ForEach(s => _scenarios.TryAdd(s.Id, s));
            }
            else
            {
                SaveChanges();
            }
        }
    }
}