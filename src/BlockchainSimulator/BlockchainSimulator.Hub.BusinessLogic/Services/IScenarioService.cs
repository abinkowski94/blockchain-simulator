using BlockchainSimulator.Hub.BusinessLogic.Model;
using BlockchainSimulator.Hub.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Hub.BusinessLogic.Services
{
    public interface IScenarioService
    {
        BaseResponse<Scenario> CreateScenario(Scenario scenario);
    }
}