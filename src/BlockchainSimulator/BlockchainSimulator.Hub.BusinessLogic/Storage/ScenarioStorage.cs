using System;
using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.DataAccess.Repositories;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public class ScenarioStorage : IScenarioStorage
    {
        private readonly IFileRepository _fileRepository;
        private readonly ConcurrentDictionary<Guid, Scenario> _scenarios;
        private readonly string _simulationFile;
        private readonly object _padlock = new object();

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

        public Scenario GetScenario(Guid scenarioId)
        {
            _scenarios.TryGetValue(scenarioId, out var result);
            return result;
        }

        public Scenario RemoveScenario(Guid scenarioId)
        {
            _scenarios.TryRemove(scenarioId, out var result);
            return result;
        }

        public List<Scenario> GetScenarios()
        {
            return _scenarios.Select(kv => kv.Value).ToList();
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