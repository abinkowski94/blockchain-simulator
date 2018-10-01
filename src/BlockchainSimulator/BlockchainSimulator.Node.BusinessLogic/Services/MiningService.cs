using BlockchainSimulator.Node.BusinessLogic.Configurations;
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

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : BaseService, IMiningService
    {
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IConsensusService _consensusService;
        private readonly IStatisticService _statisticService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBlockProvider _blockProvider;
        private readonly IBackgroundTaskQueue _queue;
        private ITransactionService _transactionService;

        public MiningService(IBlockchainRepository blockchainRepository, IConsensusService consensusService,
            IStatisticService statisticService, IBlockProvider blockProvider, IBackgroundTaskQueue queue,
            IBlockchainConfiguration blockchainConfiguration, IServiceProvider serviceProvider)
        {
            _blockchainConfiguration = blockchainConfiguration;
            _blockchainRepository = blockchainRepository;
            _consensusService = consensusService;
            _statisticService = statisticService;
            _serviceProvider = serviceProvider;
            _blockProvider = blockProvider;
            _queue = queue;
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

                while (!newBlockTask.IsCompleted)
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

        public void ReMineBlocks()
        {
            if (_transactionService == null)
            {
                _transactionService = _serviceProvider.GetService<ITransactionService>();
            }

            if (_queue.Length == 0)
            {
                var pendingTransactions = _transactionService.GetPendingTransactions().Result;
                if (pendingTransactions.Count < _blockchainConfiguration.BlockSize)
                {
                    var longestBlockchainBlocks = _blockchainRepository.GetLongestBlockchain()?.Blocks;
                    if (longestBlockchainBlocks != null)
                    {
                        var longestBlockchainTransactionsIds = longestBlockchainBlocks
                            .SelectMany(b => b.Body.Transactions).Select(t => t.Id).ToList();
                        var transactionsToReMine = _transactionService.RegisteredTransactions.Values
                            .Where(t => !longestBlockchainTransactionsIds.Contains(t.Id))
                            .Where(t => pendingTransactions.All(pt => pt.Id != t.Id)).ToList();

                        transactionsToReMine.ForEach(t => _transactionService.AddTransaction(t));
                    }
                }
            }
        }
    }
}