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
using BlockchainSimulator.Node.DataAccess.Model.Block;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IConfiguration _configuration;

        private int _currentMiningQueueLength;
        private int _maxMiningQueueLength;
        private int _miningAttemptsCount;
        private int _rejectedIncomingBlockCount;
        private TimeSpan _totalMiningQueueTime;

        public StatisticService(IBlockchainRepository blockchainRepository,
            IBlockchainConfiguration blockchainConfiguration, IConfiguration configuration)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainConfiguration = blockchainConfiguration;
            _configuration = configuration;
        }

        public void RegisterMiningAttempt()
        {
            _miningAttemptsCount++;
        }

        public void RegisterQueueTime(TimeSpan timespan)
        {
            _totalMiningQueueTime += timespan;
        }

        public void RegisterRejectedBlock()
        {
            _rejectedIncomingBlockCount++;
        }

        public void RegisterQueueLengthChange(int length)
        {
            _currentMiningQueueLength = length;

            if (_maxMiningQueueLength < length)
            {
                _maxMiningQueueLength = length;
            }
        }

        public BaseResponse<Statistic> GetStatistics()
        {
            var blockchainTree = _blockchainRepository.GetBlockchainTree();
            if (blockchainTree?.Blocks == null || !blockchainTree.Blocks.Any())
            {
                return new ErrorResponse<Statistic>("Could not retrieve statistics because blockchainTree is empty",
                    null);
            }

            var result = new Statistic();

            AddSessionConfiguration(result);
            AddMiningQueueStatistics(result);
            AddBlockchainStatistics(result, blockchainTree);

            return new SuccessResponse<Statistic>($"The statistics has been generated on: {DateTime.UtcNow}", result);
        }

        private static void AddTransactionsStatistics(BlockchainStatistics blockchainStatistics,
            BlockchainTree blockChain)
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

        private static void AddBlockchainStatistics(Statistic result, BlockchainTree blockchainTree)
        {
            result.BlockchainStatistics = new BlockchainStatistics
            {
                BlocksCount = blockchainTree.Blocks.Count,
                TotalQueueTimeForBlocks = blockchainTree.Blocks.Sum(b => b.QueueTime),
                TotalTransactionsCount = blockchainTree.Blocks.Sum(b => b.Body.TransactionCounter)
            };

            AddBlockchainTrees(result.BlockchainStatistics, blockchainTree);
            AddTransactionsStatistics(result.BlockchainStatistics, blockchainTree);
        }

        private static void AddBlockchainTrees(BlockchainStatistics blockchainStatistics, BlockchainTree blockchainTree)
        {
            var mappedBlocksInfo = blockchainTree.Blocks.Select(b => new BlockInfo
            {
                UniqueId = b.UniqueId,
                ParentUniqueId = (b as Block)?.ParentUniqueId,
                Id = b.Id,
                Nonce = b.Header.Nonce,
                TimeStamp = b.Header.TimeStamp
            }).ToList();
            blockchainStatistics.BlockInfos = mappedBlocksInfo;
        }

        private void AddMiningQueueStatistics(Statistic result)
        {
            result.MiningQueueStatistics = new MiningQueueStatistics
            {
                CurrentQueueLength = _currentMiningQueueLength,
                MaxQueueLength = _maxMiningQueueLength,
                TotalQueueTime = _totalMiningQueueTime,
                AverageQueueTime = _totalMiningQueueTime /
                                   (_maxMiningQueueLength != 0 ? _maxMiningQueueLength : 1),
                TotalMiningAttemptsCount = _miningAttemptsCount,
                RejectedIncomingBlockchainCount = _rejectedIncomingBlockCount
            };
        }

        private void AddSessionConfiguration(Statistic result)
        {
            result.BlockSize = _blockchainConfiguration.BlockSize;
            result.Target = _blockchainConfiguration.Target;
            result.Version = _blockchainConfiguration.Version;
            result.NodeType = _configuration["Node:Type"];
            result.NodeId = _configuration["Node:Id"];
        }
    }
}