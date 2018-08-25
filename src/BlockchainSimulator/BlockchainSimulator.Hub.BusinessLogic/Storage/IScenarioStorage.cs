using BlockchainSimulator.Hub.BusinessLogic.Model;

namespace BlockchainSimulator.Hub.BusinessLogic.Storage
{
    public interface IScenarioStorage
    {
        Scenario AddScenario(Scenario scenario);
        void SaveChanges();
    }
}