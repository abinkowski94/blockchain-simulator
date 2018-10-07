using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ITransactionService _transactionService;
        private readonly IConfiguration _configuration;
        private readonly IBackgroundTaskQueue _queue;
        private readonly IMiningQueue _miningQueue;

        public ConfigurationService(IConfiguration configuration, ITransactionService transactionService,
            IMiningQueue miningQueue, IBackgroundTaskQueue queue)
        {
            _configuration = configuration;
            _transactionService = transactionService;
            _miningQueue = miningQueue;
            _queue = queue;
        }

        public List<KeyValuePair<string, string>> GetConfigurationInfo()
        {
            return _configuration.AsEnumerable().Where(kv => kv.Value != null).ToList();
        }

        public BaseResponse<bool> StopAllJobs()
        {
            _transactionService.Clear();
            _miningQueue.Clear();
            _queue.Clear();

            return new SuccessResponse<bool>("The jobs has been stopped!", true);
        }
    }
}