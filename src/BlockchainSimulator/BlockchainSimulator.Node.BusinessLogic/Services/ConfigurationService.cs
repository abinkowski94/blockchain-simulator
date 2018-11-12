using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Storage;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IEncodedBlocksStorage _encodedBlocksStorage;
        private readonly ITransactionStorage _transactionStorage;
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        private readonly IBackgroundQueue _queue;

        private BlockchainNodeConfiguration _nodeConfiguration;
        private IBlockchainService _blockchainService;
        private IStatisticService _statisticService;
        private IConsensusService _consensusService;
        private IMiningQueue _miningQueue;

        private readonly object _padlock = new object();

        public ConfigurationService(IEncodedBlocksStorage encodedBlocksStorage, ITransactionStorage transactionStorage,
            IServiceProvider serviceProvider, IConfiguration configuration, IBackgroundQueue queue)
        {
            _encodedBlocksStorage = encodedBlocksStorage;
            _transactionStorage = transactionStorage;
            _serviceProvider = serviceProvider;
            _configuration = configuration;
            _queue = queue;
        }

        public BlockchainNodeConfiguration GetConfiguration()
        {
            var nodeId = _configuration["Node:Id"] ??
                         throw new ApplicationException("The id of the node must be known value");
            var nodeType = _configuration["Node:Type"] ??
                           throw new ApplicationException("The algorithm type must be known");

            if (!int.TryParse(_configuration["BlockchainConfiguration:BlockSize"], out var blockSize))
            {
                blockSize = 5;
            }

            if (!int.TryParse(_configuration["BlockchainConfiguration:EpochSize"], out var epochSize))
            {
                epochSize = 10;
            }

            if (!bool.TryParse(_configuration["Node:IsValidator"], out var nodeIsValidator))
            {
                nodeIsValidator = true;
            }

            var target = _configuration["BlockchainConfiguration:Target"] ?? "000";
            var version = _configuration["BlockchainConfiguration:Version"] ?? "PoW-v1";
            var startupValidatorsString = _configuration["BlockchainConfiguration:StartupValidators"] ?? "{}";
            var startupValidators = JsonConvert.DeserializeObject<Dictionary<string, int>>(startupValidatorsString);

            return _nodeConfiguration ?? (_nodeConfiguration = new BlockchainNodeConfiguration
            {
                BlockSize = blockSize,
                Target = target,
                Version = version,
                EpochSize = epochSize,
                StartupValidatorsWithStakes = startupValidators,
                NodeId = nodeId,
                NodeType = nodeType,
                NodeIsValidator = nodeIsValidator
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
                _consensusService.DisconnectFromNetwork();
                _statisticService.Clear();
                _blockchainService.Clear();
                _blockchainService.CreateGenesisBlockIfNotExist();

                return new SuccessResponse<bool>("The node has been cleared!", true);
            }
        }

        public BaseResponse<bool> StopAllJobs()
        {
            lock (_padlock)
            {
                _miningQueue = _miningQueue ?? _serviceProvider.GetService<IMiningQueue>();

                _miningQueue.Clear();
                _transactionStorage.Clear();
                _encodedBlocksStorage.Clear();
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