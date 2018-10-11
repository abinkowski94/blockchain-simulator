using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IConfigurationService
    {
        BaseResponse<bool> StopAllJobs();

        BaseResponse<bool> ClearNode();

        BlockchainNodeConfiguration GetConfiguration();

        BaseResponse<bool> ChangeConfiguration(BlockchainNodeConfiguration configuration);
    }
}