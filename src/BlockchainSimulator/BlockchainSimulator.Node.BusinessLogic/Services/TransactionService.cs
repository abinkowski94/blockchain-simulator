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
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ConcurrentDictionary<string, Transaction> _pendingTransactions;
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IBlockchainService _blockchainService;
        private readonly IMiningService _miningService;
        private readonly IConfiguration _configuration;
        private readonly IMiningQueue _miningQueue;
        private readonly object _padlock = new object();

        public ConcurrentDictionary<string, Transaction> RegisteredTransactions { get; }

        public TransactionService(IBlockchainService blockchainService, IMiningService miningService,
            IBlockchainConfiguration blockchainConfiguration, IMiningQueue queue, IConfiguration configuration)
        {
            _pendingTransactions = new ConcurrentDictionary<string, Transaction>();
            _blockchainConfiguration = blockchainConfiguration;
            _blockchainService = blockchainService;
            _miningService = miningService;
            _configuration = configuration;
            _miningQueue = queue;

            RegisteredTransactions = new ConcurrentDictionary<string, Transaction>();
        }

        public BaseResponse<Transaction> AddTransaction(Transaction transaction)
        {
            lock (_padlock)
            {
                transaction.Id = transaction.Id ?? $"{Guid.NewGuid().ToString()}-{_configuration["Node:Id"]}";
                transaction.RegistrationTime = transaction.RegistrationTime ?? DateTime.UtcNow;
                transaction.TransactionDetails = null;

                if (!_pendingTransactions.TryAdd(transaction.Id, transaction))
                {
                    return new ErrorResponse<Transaction>(
                        $"Could not add the transaction: {transaction.Id} to the pending list", transaction);
                }

                RegisteredTransactions.TryAdd(transaction.Id, transaction);
                if (_pendingTransactions.Count % _blockchainConfiguration.BlockSize != 0)
                {
                    return new SuccessResponse<Transaction>("The transaction has been added to pending list",
                        transaction);
                }

                MineTransactions();
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

        private void MineTransactions()
        {
            var enqueueTime = DateTime.UtcNow;
            var transactions = _pendingTransactions.Values.OrderByDescending(t => t.Fee)
                .Take(_blockchainConfiguration.BlockSize).ToList();
            transactions.ForEach(t => _pendingTransactions.TryRemove(t.Id, out _));
            _miningQueue.QueueMiningTask(token =>
                new Task(() => _miningService.MineBlock(transactions, enqueueTime, token), token));
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
    }
}