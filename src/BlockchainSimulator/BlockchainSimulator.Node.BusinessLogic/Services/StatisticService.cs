using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Statistics;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IConfiguration _configuration;
        private readonly List<List<BlockInfo>> blockchainBranches;
        private int abandonedBlocksCount;
        private int currentMiningQueueLength;
        private int maxMiningQueueLength;
        private int miningAttemptsCount;
        private int rejectedIncomingBlockchainCount;
        private TimeSpan totalMiningQueueTime;

        public StatisticService(IBlockchainRepository blockchainRepository,
            IBlockchainConfiguration blockchainConfiguration, IConfiguration configuration)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainConfiguration = blockchainConfiguration;
            _configuration = configuration;

            blockchainBranches = new List<List<BlockInfo>>();
            abandonedBlocksCount = 0;
            currentMiningQueueLength = 0;
            maxMiningQueueLength = 0;
            miningAttemptsCount = 0;
            rejectedIncomingBlockchainCount = 0;
            totalMiningQueueTime = TimeSpan.FromSeconds(0);
        }

        public void AddBlockchainBranch(Blockchain incomingBlockchain)
        {
            blockchainBranches.Add(incomingBlockchain.Blocks.Select(b => new BlockInfo
            {
                Id = b.Id,
                TimeStamp = b.Header.TimeStamp,
                Nonce = b.Header.Nonce
            }).ToList());
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

        public void RegisterAbandonedBlock()
        {
            abandonedBlocksCount++;
        }

        public void RegisterMiningAttempt()
        {
            miningAttemptsCount++;
        }

        public void RegisterQueueLengthChange(int length)
        {
            currentMiningQueueLength = length;

            if (maxMiningQueueLength < length)
            {
                maxMiningQueueLength = length;
            }
        }

        public void RegisterQueueTime(TimeSpan timespan)
        {
            totalMiningQueueTime += timespan;
        }

        public void RegisterRejectedBlockchain()
        {
            rejectedIncomingBlockchainCount++;
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
            blockchainStatistics.BlockchainBranches = blockchainBranches;
        }

        private void AddMiningQueueStatistics(Statistic result)
        {
            result.MiningQueueStatistics = new MiningQueueStatistics
            {
                CurrentQueueLength = currentMiningQueueLength,
                MaxQueueLength = maxMiningQueueLength,
                TotalQueueTime = totalMiningQueueTime,
                AverageQueueTime = totalMiningQueueTime /
                                   (maxMiningQueueLength != 0 ? maxMiningQueueLength : 1),
                AbandonedBlocksCount = abandonedBlocksCount,
                TotalMiningAttemptsCount = miningAttemptsCount,
                RejectedIncomingBlockchainCount = rejectedIncomingBlockchainCount
            };
        }

        private void AddSessionConfiguration(Statistic result)
        {
            result.BlockSize = _blockchainConfiguration.BlockSize;
            result.Target = _blockchainConfiguration.Target;
            result.Version = _blockchainConfiguration.Version;
            result.NodeType = _configuration["Node:Type"];
        }
    }
}