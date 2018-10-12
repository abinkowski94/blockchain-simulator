using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using BlockchainSimulator.Node.BusinessLogic.Storage;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IEncodedBlocksStorage _encodedBlocksStorage;
        private readonly ITransactionStorage _transactionStorage;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBackgroundQueue _queue;
        private readonly IMiningQueue _miningQueue;
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly object _padlock = new object();

        private BlockchainNodeConfiguration _nodeConfiguration;
        private IStatisticService _statisticService;
        private IConsensusService _consensusService;

        public ConfigurationService(IConfiguration configuration, IMiningQueue miningQueue,
            IBackgroundQueue queue, IServiceProvider serviceProvider, IBlockchainRepository blockchainRepository,
            ITransactionStorage transactionStorage, IEncodedBlocksStorage encodedBlocksStorage)
        {
            _blockchainRepository = blockchainRepository;
            _transactionStorage = transactionStorage;
            _encodedBlocksStorage = encodedBlocksStorage;
            _configuration = configuration;
            _miningQueue = miningQueue;
            _queue = queue;
            _serviceProvider = serviceProvider;
        }

        public BlockchainNodeConfiguration GetConfiguration()
        {
            return _nodeConfiguration ?? (_nodeConfiguration = new BlockchainNodeConfiguration
            {
                BlockSize = Convert.ToInt32(_configuration["BlockchainConfiguration:BlockSize"]),
                Target = _configuration["BlockchainConfiguration:Target"],
                Version = _configuration["BlockchainConfiguration:Version"],
                NodeId = _configuration["Node:Id"],
                NodeType = _configuration["Node:Type"]
            });
        }

        public BaseResponse<bool> ClearNode()
        {
            lock (_padlock)
            {
                _consensusService = _consensusService ?? _serviceProvider.GetService<IConsensusService>();
                _statisticService = _statisticService ?? _serviceProvider.GetService<IStatisticService>();

                StopAllJobs();
                _blockchainRepository.Clear();
                _consensusService.DisconnectFromNetwork();
                _statisticService.Clear();

                return new SuccessResponse<bool>("The node has been cleared!", true);
            }
        }

        public BaseResponse<bool> StopAllJobs()
        {
            lock (_padlock)
            {
                _transactionStorage.Clear();
                _encodedBlocksStorage.Clear();
                _miningQueue.Clear();
                _queue.Clear();

                return new SuccessResponse<bool>("The jobs has been stopped!", true);
            }
        }

        public BaseResponse<bool> ChangeConfiguration(BlockchainNodeConfiguration configuration)
        {
            _nodeConfiguration = configuration;
            ClearNode();

            return new SuccessResponse<bool>("The configuration has been changed", true);
        }
    }
}