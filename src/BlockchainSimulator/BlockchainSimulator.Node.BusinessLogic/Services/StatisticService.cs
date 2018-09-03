using System;
using System.Collections.Generic;
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
        private readonly IConsensusService _consensusService;

        public StatisticService(IBlockchainRepository blockchainRepository, IMiningQueue miningQueue,
            IBlockchainConfiguration blockchainConfiguration, IConfiguration configuration,
            IMiningService miningService, IConsensusService consensusService)
        {
            _blockchainRepository = blockchainRepository;
            _miningQueue = miningQueue;
            _blockchainConfiguration = blockchainConfiguration;
            _configuration = configuration;
            _miningService = miningService;
            _consensusService = consensusService;
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
            AddMiningQueueStatistics(result);
            AddBlockchainStatistics(result, blockChain);

            return new SuccessResponse<Statistic>($"The statistics has been generated on: {DateTime.UtcNow}", result);
        }

        private void AddSessionConfiguration(Statistic result)
        {
            result.BlockSize = _blockchainConfiguration.BlockSize;
            result.Target = _blockchainConfiguration.Target;
            result.Version = _blockchainConfiguration.Version;
            result.NodeType = _configuration["Node:Type"];
        }

        private void AddMiningQueueStatistics(Statistic result)
        {
            result.MiningQueueStatistics = new MiningQueueStatistics
            {
                CurrentQueueLength = _miningQueue.Length,
                MaxQueueLength = _miningQueue.MaxQueueLength,
                TotalQueueTime = _miningQueue.TotalQueueTime,
                AverageQueueTime = _miningQueue.TotalQueueTime /
                                   (_miningQueue.MaxQueueLength != 0 ? _miningQueue.MaxQueueLength : 1),
                AbandonedBlocksCount = _miningService.AbandonedBlocksCount,
                TotalMiningAttemptsCount = _miningService.MiningAttemptsCount,
                RejectedIncomingBlockchainCount = _consensusService.RejectedIncomingBlockchainCount
            };
        }

        private void AddBlockchainStatistics(Statistic result, Blockchain blockChain)
        {
            result.BlockchainStatistics = new BlockchainStatistics
            {
                BlocksCount = blockChain.Blocks.Count,
                TotalQueueTimeForBlocks = blockChain.Blocks.Sum(b => b.QueueTime),
                TotalTransactionsCount = blockChain.Blocks.Sum(b => b.Body.TransactionCounter)
            };

            AddBlockchainTrees(result.BlockchainStatistics);
            AddTransactionsStatistics(result.BlockchainStatistics, blockChain);
        }

        private void AddBlockchainTrees(BlockchainStatistics blockchainStatistics)
        {
            blockchainStatistics.BlockchainBranches = _consensusService.BlockchainBranches;
        }

        private static void AddTransactionsStatistics(BlockchainStatistics blockchainStatistics, Blockchain blockChain)
        {
            blockchainStatistics.TransactionsStatistics = new List<TransactionStatistics>();

            blockChain.Blocks.ForEach(b =>
            {
                b.Body.Transactions.ForEach(t =>
                {
                    blockchainStatistics.TransactionsStatistics.Add(new TransactionStatistics
                    {
                        BlockId = b.Id,
                        BlockQueueTime = b.QueueTime,
                        TransactionRegistrationTime = t.RegistrationTime,
                        TransactionId = t.Id,
                        TransactionFee = t.Fee,
                        TransactionConfirmationTime = b.Header.TimeStamp - t.RegistrationTime
                    });
                });
            });
        }
    }
}