using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Node.BusinessLogic.Storage;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : BaseService, IMiningService
    {
        private readonly ITransactionStorage _transactionStorage;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IConsensusService _consensusService;
        private readonly IStatisticService _statisticService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBlockProvider _blockProvider;
        private readonly IMiningQueue _miningQueue;
        private readonly IBackgroundQueue _backgroundQueue;

        private ITransactionService _transactionService;

        public MiningService(IBlockchainRepository blockchainRepository, IConsensusService consensusService,
            IStatisticService statisticService, IServiceProvider serviceProvider, IBlockProvider blockProvider,
            IMiningQueue miningQueue, IBackgroundQueue backgroundQueue, IConfigurationService configurationService,
            ITransactionStorage transactionStorage) : base(configurationService)
        {
            _blockchainRepository = blockchainRepository;
            _consensusService = consensusService;
            _statisticService = statisticService;
            _serviceProvider = serviceProvider;
            _blockProvider = blockProvider;
            _miningQueue = miningQueue;
            _backgroundQueue = backgroundQueue;
            _transactionStorage = transactionStorage;
        }

        public void MineBlock(HashSet<Transaction> transactions, DateTime enqueueTime, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                _statisticService.RegisterMiningAttempt();

                var cancellationTokenSource = new CancellationTokenSource();
                var lastBlock = LocalMapper.Map<BlockBase>(_blockchainRepository.GetLastBlock());
                var newBlockTask = _blockProvider.CreateBlock(transactions, enqueueTime, lastBlock,
                    cancellationTokenSource.Token);

                while (!newBlockTask.IsCompleted || newBlockTask.IsCompleted && newBlockTask.IsCanceled)
                {
                    if (lastBlock?.UniqueId != _blockchainRepository.GetLastBlock()?.UniqueId)
                    {
                        cancellationTokenSource.Cancel();
                    }

                    if (newBlockTask.IsCanceled)
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        lastBlock = LocalMapper.Map<BlockBase>(_blockchainRepository.GetLastBlock());
                        newBlockTask = _blockProvider.CreateBlock(transactions, enqueueTime, lastBlock,
                            cancellationTokenSource.Token);

                        _statisticService.RegisterAbandonedBlock();
                    }
                }

                var result = _consensusService.AcceptBlock(newBlockTask.Result);
                if (!result.IsSuccess)
                {
                    _statisticService.RegisterAbandonedBlock();
                }
            }
        }

        public void ReMineAndSynchronizeBlocks()
        {
            if (!_miningQueue.IsWorking && !_backgroundQueue.IsWorking)
            {
                var pendingTransactions = _transactionStorage.PendingTransactions;
                if (pendingTransactions.Count < BlockchainNodeConfiguration.BlockSize)
                {
                    var longestBlockchainBlocks = _blockchainRepository.GetLongestBlockchain()?.Blocks;
                    if (longestBlockchainBlocks != null)
                    {
                        var longestBlockchainTransactionsIds = longestBlockchainBlocks
                            .SelectMany(b => b.Body.Transactions).Select(t => t.Id).ToList();

                        var treeTransactions = _blockchainRepository.GetBlockchainTree().Blocks
                            .SelectMany(b => b.Body.Transactions).Select(t => LocalMapper.Map<Transaction>(t));

                        treeTransactions.ForEach(t => _transactionStorage.RegisteredTransactions.TryAdd(t.Id, t));

                        var transactionsToReMine = _transactionStorage.RegisteredTransactions.Values
                            .Where(t => !longestBlockchainTransactionsIds.Contains(t.Id))
                            .Where(t => pendingTransactions.All(pt => pt.Key != t.Id)).ToList();

                        if (transactionsToReMine.Any())
                        {
                            _transactionService =
                                _transactionService ?? _serviceProvider.GetService<ITransactionService>();
                            _transactionService.AddTransactions(transactionsToReMine);
                        }
                        else
                        {
                            var synchronizationResponse = _consensusService.SynchronizeWithOtherNodes();
                            if (synchronizationResponse.IsSuccess && synchronizationResponse.Result)
                            {
                                ReMineAndSynchronizeBlocks();
                            }
                        }
                    }
                }
            }
        }
    }
}