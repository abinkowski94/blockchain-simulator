using System;
using System.Linq;
using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Statistics;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IMiningQueue _miningQueue;

        public StatisticService(IBlockchainRepository blockchainRepository, IMiningQueue miningQueue)
        {
            _blockchainRepository = blockchainRepository;
            _miningQueue = miningQueue;
        }

        public BaseResponse<Statistic> GetStatistics()
        {
            var blockChain = _blockchainRepository.GetBlockchain();
            if (blockChain?.Blocks == null || !blockChain.Blocks.Any())
            {
                return new ErrorResponse<Statistic>("Could not retrieve statistics because blockchain is empty", null);
            }

            var result = new Statistic();

            AddBlockchainStatistics(result, blockChain);
            AddQueueStatistics(result);

            return new SuccessResponse<Statistic>($"The statistics has been generated on: {DateTime.UtcNow}", result);
        }

        private static void AddBlockchainStatistics(Statistic result, Blockchain blockChain)
        {
            result.BlockchainStatistics = new BlockchainStatistics
            {
                BlocksCount = blockChain.Blocks.Count,
                TotalQueueTimeForBlocks = blockChain.Blocks.Sum(b => b.QueueTime)
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
                                   (_miningQueue.MaxQueueLength != 0 ? _miningQueue.MaxQueueLength : 1)
            };
        }
    }
}