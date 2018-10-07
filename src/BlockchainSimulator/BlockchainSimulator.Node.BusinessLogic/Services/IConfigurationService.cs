using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IConfigurationService
    {
        List<KeyValuePair<string, string>> GetConfigurationInfo();
        
        BaseResponse<bool> StopAllJobs();
    }
}