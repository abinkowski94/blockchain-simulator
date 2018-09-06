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
        private readonly IBlockchainService _blockchainService;
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IMiningService _miningService;
        private readonly ConcurrentDictionary<string, Transaction> _pendingTransactions;
        private readonly IMiningQueue _queue;
        private readonly IConfiguration _configuration;
        private readonly object _padlock = new object();

        public TransactionService(IBlockchainService blockchainService, IMiningService miningService,
            IBlockchainConfiguration blockchainConfiguration, IMiningQueue queue, IConfiguration configuration)
        {
            _pendingTransactions = new ConcurrentDictionary<string, Transaction>();
            _blockchainConfiguration = blockchainConfiguration;
            _blockchainService = blockchainService;
            _miningService = miningService;
            _queue = queue;
            _configuration = configuration;
        }

        public BaseResponse<Transaction> AddTransaction(Transaction transaction)
        {
            lock (_padlock)
            {
                if (transaction.Id != null && !transaction.Id.EndsWith(_configuration["Node:Id"]))
                {
                    return new ErrorResponse<Transaction>("Can not mine others node transaction", transaction);
                }
                
                transaction.Id = transaction.Id ?? $"{Guid.NewGuid().ToString()}-{_configuration["Node:Id"]}";
                transaction.RegistrationTime = transaction.Id != null ? DateTime.UtcNow : transaction.RegistrationTime;
                transaction.TransactionDetails = null;

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
                _queue.QueueMiningTask(token => new Task(() =>
                {
                    var transactions = _pendingTransactions.Values.OrderByDescending(t => t.Fee)
                        .Take(_blockchainConfiguration.BlockSize).ToList();
                    transactions.ForEach(t => _pendingTransactions.TryRemove(t.Id, out _));
                    _miningService.MineBlock(transactions, enqueueTime, token);
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
    }
}