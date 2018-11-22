using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Hubs;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Hubs;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Statistics;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Node.BusinessLogic.Storage;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class StatisticService : IStatisticService
    {
        private readonly IHubContext<SimulationHub, ISimulationClient> _simulationHubContext;
        private readonly IBlockchainService _blockchainService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IStakingStorage _stakingStorage;
        private readonly object _padlock = new object();

        private int _rejectedIncomingBlockCount;
        private int _currentMiningQueueLength;
        private int _maxMiningQueueLength;
        private int _miningAttemptsCount;
        private int _abandonedBlockCount;
        private bool _isWorking;
        private TimeSpan _totalMiningQueueTime;

        private BlockchainNodeConfiguration BlockchainNodeConfiguration =>
            _serviceProvider.GetService<IConfigurationService>()?.GetConfiguration();

        public StatisticService(IHubContext<SimulationHub, ISimulationClient> simulationHubContext,
            IBlockchainService blockchainService, IServiceProvider serviceProvider, IStakingStorage stakingStorage)
        {
            _simulationHubContext = simulationHubContext;
            _blockchainService = blockchainService;
            _serviceProvider = serviceProvider;
            _stakingStorage = stakingStorage;
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

        public void RegisterAbandonedBlock()
        {
            _abandonedBlockCount++;
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
            var blockchainTree = _blockchainService.GetBlockchainTree();
            if (blockchainTree?.Blocks == null || !blockchainTree.Blocks.Any())
            {
                return new ErrorResponse<Statistic>("Could not retrieve statistics because blockchain tree is empty",
                    null);
            }

            var blockchain = _blockchainService.GetLongestBlockchain();
            var result = new Statistic
            {
                Epochs = _stakingStorage.Epochs.Values.ToList()
            };

            AddSessionConfiguration(result);
            AddMiningQueueStatistics(result);
            AddBlockchainStatistics(result, blockchainTree, blockchain);

            return new SuccessResponse<Statistic>($"The statistics has been generated on: {DateTime.UtcNow}", result);
        }

        public void RegisterWorkingStatus(bool isWorking)
        {
            lock (_padlock)
            {
                if (_isWorking != isWorking)
                {
                    _isWorking = isWorking;
                    _simulationHubContext.Clients.All.ChangeWorkingStatus(_isWorking);
                }
            }
        }

        public void Clear()
        {
            _rejectedIncomingBlockCount = 0;
            _currentMiningQueueLength = 0;
            _maxMiningQueueLength = 0;
            _miningAttemptsCount = 0;
            _abandonedBlockCount = 0;
            _isWorking = false;
            _totalMiningQueueTime = TimeSpan.Zero;
        }

        private static void AddTransactionsStatistics(BlockchainStatistics blockchainStatistics,
                            BlockchainTree blockchain)
        {
            blockchainStatistics.TransactionsStatistics = new List<TransactionStatistics>();

            blockchain.Blocks.ForEach(b =>
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

        private static void AddBlockchainStatistics(Statistic result, BlockchainTree blockchainTree,
            BlockchainTree blockchain)
        {
            result.BlockchainStatistics = new BlockchainStatistics
            {
                BlocksCount = blockchain.Blocks.Count,
                TotalQueueTimeForBlocks = blockchain.Blocks.Sum(b => b.QueueTime),
                TotalTransactionsCount = blockchain.Blocks.Sum(b => b.Body.TransactionCounter)
            };

            AddBlockchainTrees(result.BlockchainStatistics, blockchainTree);
            AddTransactionsStatistics(result.BlockchainStatistics, blockchain);
        }

        private static void AddBlockchainTrees(BlockchainStatistics blockchainStatistics, BlockchainTree blockchainTree)
        {
            blockchainStatistics.BlockInfos = blockchainTree.Blocks.Select(b => new BlockInfo
            {
                UniqueId = b.UniqueId,
                ParentUniqueId = (b as Block)?.ParentUniqueId,
                Id = b.Id,
                Nonce = b.Header.Nonce,
                TimeStamp = b.Header.TimeStamp
            }).ToList();
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
                RejectedIncomingBlockchainCount = _rejectedIncomingBlockCount,
                AbandonedBlocksCount = _abandonedBlockCount
            };
        }

        private void AddSessionConfiguration(Statistic result)
        {
            result.BlockSize = BlockchainNodeConfiguration.BlockSize;
            result.Target = BlockchainNodeConfiguration.Target;
            result.Version = BlockchainNodeConfiguration.Version;
            result.NodeType = BlockchainNodeConfiguration.NodeType;
            result.NodeId = BlockchainNodeConfiguration.NodeId;
        }
    }
}