using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Statistics;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IStatisticService
    {
        BaseResponse<Statistic> GetStatistics();
    }
}