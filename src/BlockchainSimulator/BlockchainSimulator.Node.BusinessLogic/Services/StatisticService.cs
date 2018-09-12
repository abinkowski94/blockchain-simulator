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
        private readonly List<List<BlockInfo>> _blockchainBranches;
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IConfiguration _configuration;
        private int _abandonedBlocksCount;
        private int _currentMiningQueueLength;
        private int _maxMiningQueueLength;
        private int _miningAttemptsCount;
        private int _rejectedIncomingBlockchainCount;
        private TimeSpan _totalMiningQueueTime;

        public StatisticService(IBlockchainRepository blockchainRepository,
            IBlockchainConfiguration blockchainConfiguration, IConfiguration configuration)
        {
            _blockchainRepository = blockchainRepository;
            _blockchainConfiguration = blockchainConfiguration;
            _configuration = configuration;

            _blockchainBranches = new List<List<BlockInfo>>();
            _abandonedBlocksCount = 0;
            _currentMiningQueueLength = 0;
            _maxMiningQueueLength = 0;
            _miningAttemptsCount = 0;
            _rejectedIncomingBlockchainCount = 0;
            _totalMiningQueueTime = TimeSpan.FromSeconds(0);
        }

        public void AddBlockchainBranch(BlockchainTree incomingBlockchainTree)
        {
            if (incomingBlockchainTree?.Blocks != null
                && _blockchainBranches.All(b => b.Count != incomingBlockchainTree.Blocks.Count))
            {
                _blockchainBranches.Add(incomingBlockchainTree.Blocks.Select(b => new BlockInfo
                {
                    Id = b.Id,
                    TimeStamp = b.Header.TimeStamp,
                    Nonce = b.Header.Nonce
                }).ToList());
            }
        }

        public BaseResponse<Statistic> GetStatistics()
        {
            var blockChain = _blockchainRepository.GetBlockchainTree();
            if (blockChain?.Blocks == null || !blockChain.Blocks.Any())
            {
                return new ErrorResponse<Statistic>("Could not retrieve statistics because blockchainTree is empty", null);
            }

            var result = new Statistic();

            AddSessionConfiguration(result);
            AddMiningQueueStatistics(result);
            AddBlockchainStatistics(result, blockChain);

            return new SuccessResponse<Statistic>($"The statistics has been generated on: {DateTime.UtcNow}", result);
        }

        public void RegisterAbandonedBlock()
        {
            _abandonedBlocksCount++;
        }

        public void RegisterMiningAttempt()
        {
            _miningAttemptsCount++;
        }

        public void RegisterQueueLengthChange(int length)
        {
            _currentMiningQueueLength = length;

            if (_maxMiningQueueLength < length)
            {
                _maxMiningQueueLength = length;
            }
        }

        public void RegisterQueueTime(TimeSpan timespan)
        {
            _totalMiningQueueTime += timespan;
        }

        public void RegisterRejectedBlockchain()
        {
            _rejectedIncomingBlockchainCount++;
        }

        private static void AddTransactionsStatistics(BlockchainStatistics blockchainStatistics, BlockchainTree blockChain)
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

        private void AddBlockchainStatistics(Statistic result, BlockchainTree blockChain)
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
            blockchainStatistics.BlockchainBranches = _blockchainBranches;
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
                AbandonedBlocksCount = _abandonedBlocksCount,
                TotalMiningAttemptsCount = _miningAttemptsCount,
                RejectedIncomingBlockchainCount = _rejectedIncomingBlockchainCount
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