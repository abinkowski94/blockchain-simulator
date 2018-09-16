using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private Task _reMiningTask;
        private readonly ConcurrentDictionary<string, Transaction> _registeredTransactions;
        private readonly ConcurrentDictionary<string, Transaction> _pendingTransactions;
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IBlockchainService _blockchainService;
        private readonly IMiningService _miningService;
        private readonly IConfiguration _configuration;
        private readonly IMiningQueue _miningQueue;
        private readonly object _padlock = new object();

        public TransactionService(IBlockchainService blockchainService, IMiningService miningService,
            IBlockchainConfiguration blockchainConfiguration, IMiningQueue queue, IConfiguration configuration,
            IBlockchainRepository blockchainRepository, IBackgroundTaskQueue backgroundTaskQueue)
        {
            _registeredTransactions = new ConcurrentDictionary<string, Transaction>();
            _pendingTransactions = new ConcurrentDictionary<string, Transaction>();
            _blockchainConfiguration = blockchainConfiguration;
            _blockchainService = blockchainService;
            _miningService = miningService;
            _miningQueue = queue;
            _configuration = configuration;
            _blockchainRepository = blockchainRepository;
            _backgroundTaskQueue = backgroundTaskQueue;
        }

        public BaseResponse<Transaction> AddTransaction(Transaction transaction)
        {
            lock (_padlock)
            {
                transaction.Id = transaction.Id ?? $"{Guid.NewGuid().ToString()}-{_configuration["Node:Id"]}";
                transaction.RegistrationTime = transaction.RegistrationTime ?? DateTime.UtcNow;
                transaction.TransactionDetails = null;

                _registeredTransactions.TryAdd(transaction.Id, transaction);

                if (!_pendingTransactions.TryAdd(transaction.Id, transaction))
                {
                    return new ErrorResponse<Transaction>(
                        $"Could not add the transaction: {transaction.Id} to the pending list", transaction);
                }

                if (_pendingTransactions.Count % _blockchainConfiguration.BlockSize != 0)
                {
                    return new SuccessResponse<Transaction>("The transaction has been added to pending list",
                        transaction);
                }

                // Launches mining
                var enqueueTime = DateTime.UtcNow;
                _miningQueue.QueueMiningTask(token => new Task(() =>
                {
                    var transactions = _pendingTransactions.Values.OrderByDescending(t => t.Fee)
                        .Take(_blockchainConfiguration.BlockSize).ToList();
                    transactions.ForEach(t => _pendingTransactions.TryRemove(t.Id, out _));

                    _miningService.MineBlock(transactions, enqueueTime, token);
                    ReMineTransactions();
                }, token));

                return new SuccessResponse<Transaction>("The transaction has been added and processing has started",
                    transaction);
            }
        }

        public BaseResponse<List<Transaction>> GetPendingTransactions()
        {
            var result = _pendingTransactions.Values.OrderByDescending(t => t.Fee).ToList();
            var message =
                $"Pending transactions count: {_pendingTransactions.Count}/{_blockchainConfiguration.BlockSize}";

            return new SuccessResponse<List<Transaction>>(message, result);
        }

        public BaseResponse<Transaction> GetTransaction(string id)
        {
            var blockchainResponse = _blockchainService.GetBlockchain();
            if (!blockchainResponse.IsSuccess)
            {
                return new ErrorResponse<Transaction>("An error occured while reading blockchain from local storage!",
                    null, blockchainResponse.Message);
            }

            var result = FindAndFillTransactionData(id, blockchainResponse.Result);
            if (result == null)
            {
                return new ErrorResponse<Transaction>($"Could not find the transaction id: {id}", null);
            }

            return new SuccessResponse<Transaction>("The transaction has been found", result);
        }

        private static Transaction FindAndFillTransactionData(string transactionId, BlockBase block)
        {
            var counter = 0;
            while (block != null)
            {
                var transaction = block.Body.Transactions.FirstOrDefault(t => t.Id == transactionId);
                if (transaction != null)
                {
                    transaction.TransactionDetails = new TransactionDetails
                    {
                        BlockId = block.Id,
                        BlocksBehind = counter,
                        IsConfirmed = counter > 0
                    };
                    return transaction;
                }

                block = (block as Block)?.Parent;
                counter++;
            }

            return null;
        }

        public void ReMineTransactions()
        {
            _reMiningTask = _reMiningTask ?? Task.Run(() =>
            {
                while (_miningQueue.Length > 0 || _backgroundTaskQueue.Length > 0 ||
                       _pendingTransactions.Count > _blockchainConfiguration.BlockSize)
                {
                    // Waiting loop
                }

                if (_pendingTransactions.Count < _blockchainConfiguration.BlockSize)
                {
                    var longestBlockchainBranch = _blockchainRepository.GetLongestBlockchain();
                    if (longestBlockchainBranch == null)
                    {
                        return;
                    }

                    var longestBlockchainBranchIds = longestBlockchainBranch.Blocks.SelectMany(b => b.Body.Transactions)
                        .Select(t => t.Id).ToList();

                    _registeredTransactions.Values.Where(t => !longestBlockchainBranchIds.Contains(t.Id))
                        .ForEach(t => AddTransaction(t));
                }

                _reMiningTask = null;
            });
        }
    }
}