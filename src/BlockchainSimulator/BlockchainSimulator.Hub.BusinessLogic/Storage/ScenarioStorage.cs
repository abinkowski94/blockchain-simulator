using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.DataAccess.Repositories;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    // TODO: Make it as singleton
    public class ScenarioStorage : IScenarioStorage
    {
        private readonly IFileRepository _fileRepository;
        private readonly ConcurrentBag<Scenario> _scenarios;
        private readonly string _simulationFile;

        public ScenarioStorage(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _scenarios = new ConcurrentBag<Scenario>();
            _simulationFile = "simulations.json";
            PreloadScenarios();
        }

        public Scenario AddScenario(Scenario scenario)
        {
            _scenarios.Add(scenario);
            return scenario;
        }

        public void SaveChanges()
        {
            var data = JsonConvert.SerializeObject(_scenarios.ToList());
            _fileRepository.SaveFile(data, _simulationFile);
        }

        private void PreloadScenarios()
        {
            var data = _fileRepository.GetFile(_simulationFile);
            var scenariosList = JsonConvert.DeserializeObject<List<Scenario>>(data);
            scenariosList.ForEach(s => _scenarios.Add(s));
        }
    }
}