using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using BlockchainSimulator.Node.BusinessLogic.Storage;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IEncodedBlocksStorage _encodedBlocksStorage;
        private readonly ITransactionStorage _transactionStorage;
        private readonly IBackgroundQueue _queue;
        private readonly IBlockchainRepository _blockchainRepository;

        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;
        private readonly object _padlock = new object();

        private BlockchainNodeConfiguration _nodeConfiguration;
        private IStatisticService _statisticService;
        private IConsensusService _consensusService;
        private IMiningQueue _miningQueue;
        private IBlockchainService _blockchainService;

        public ConfigurationService(IConfiguration configuration, IBackgroundQueue queue,
            IServiceProvider serviceProvider, ITransactionStorage transactionStorage,
            IEncodedBlocksStorage encodedBlocksStorage, IBlockchainRepository blockchainRepository)
        {
            _transactionStorage = transactionStorage;
            _encodedBlocksStorage = encodedBlocksStorage;
            _blockchainRepository = blockchainRepository;
            _configuration = configuration;
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
                _blockchainService = _blockchainService ?? _serviceProvider.GetService<IBlockchainService>();

                StopAllJobs();
                _blockchainService.Clear();
                _consensusService.DisconnectFromNetwork();
                _statisticService.Clear();

                return new SuccessResponse<bool>("The node has been cleared!", true);
            }
        }

        public BaseResponse<bool> StopAllJobs()
        {
            lock (_padlock)
            {
                _miningQueue = _miningQueue ?? _serviceProvider.GetService<IMiningQueue>();

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
            _blockchainRepository.CreateGenesisBlock(_configuration);

            return new SuccessResponse<bool>("The configuration has been changed", true);
        }
    }
}