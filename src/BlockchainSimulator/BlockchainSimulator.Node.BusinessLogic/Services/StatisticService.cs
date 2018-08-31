using System;
using System.Linq;
using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Statistics;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IMiningQueue _miningQueue;
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IConfiguration _configuration;
        private readonly IMiningService _miningService;

        public StatisticService(IBlockchainRepository blockchainRepository, IMiningQueue miningQueue,
            IBlockchainConfiguration blockchainConfiguration, IConfiguration configuration,
            IMiningService miningService)
        {
            _blockchainRepository = blockchainRepository;
            _miningQueue = miningQueue;
            _blockchainConfiguration = blockchainConfiguration;
            _configuration = configuration;
            _miningService = miningService;
        }

        public BaseResponse<Statistic> GetStatistics()
        {
            var blockChain = _blockchainRepository.GetBlockchain();
            if (blockChain?.Blocks == null || !blockChain.Blocks.Any())
            {
                return new ErrorResponse<Statistic>("Could not retrieve statistics because blockchain is empty", null);
            }

            var result = new Statistic();

            AddSessionConfiguration(result);
            AddBlockchainStatistics(result, blockChain);
            AddQueueStatistics(result);

            return new SuccessResponse<Statistic>($"The statistics has been generated on: {DateTime.UtcNow}", result);
        }

        private void AddSessionConfiguration(Statistic result)
        {
            result.BlockSize = _blockchainConfiguration.BlockSize;
            result.Target = _blockchainConfiguration.Target;
            result.Version = _blockchainConfiguration.Version;
            result.NodeType = _configuration["Node:Type"];
        }

        private static void AddBlockchainStatistics(Statistic result, Blockchain blockChain)
        {
            result.BlockchainStatistics = new BlockchainStatistics
            {
                BlocksCount = blockChain.Blocks.Count,
                TotalQueueTimeForBlocks = blockChain.Blocks.Sum(b => b.QueueTime),
                TotalTransactionsCount = blockChain.Blocks.Sum(b => b.Body.TransactionCounter)
            };
        }

        private void AddQueueStatistics(Statistic result)
        {
            result.MiningQueueStatistics = new MiningQueueStatistics
            {
                CurrentQueueLength = _miningQueue.Length,
                MaxQueueLength = _miningQueue.MaxQueueLength,
                TotalQueueTime = _miningQueue.TotalQueueTime,
                AverageQueueTime = _miningQueue.TotalQueueTime /
                                   (_miningQueue.MaxQueueLength != 0 ? _miningQueue.MaxQueueLength : 1),
                AbandonedBlocksCount = _miningService.AbandonedBlocksCount,
                TotalMiningAttemptsCount = _miningService.MiningAttemptsCount
            };
        }
    }
}