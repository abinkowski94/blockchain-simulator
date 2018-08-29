using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Queues.MiningQueue;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ConcurrentDictionary<string, Transaction> _pendingTransactions;
        private readonly IBlockchainConfiguration _configuration;
        private readonly IBlockchainService _blockchainService;
        private readonly IMiningService _miningService;
        private readonly IMiningQueue _queue;

        public TransactionService(IBlockchainService blockchainService, IMiningService miningService,
            IBlockchainConfiguration configuration, IMiningQueue queue)
        {
            _pendingTransactions = new ConcurrentDictionary<string, Transaction>();
            _configuration = configuration;
            _blockchainService = blockchainService;
            _miningService = miningService;
            _queue = queue;
        }

        public BaseResponse<Transaction> AddTransaction(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid().ToString();
            transaction.RegistrationTime = DateTime.UtcNow;
            transaction.TransactionDetails = null;

            if (!_pendingTransactions.TryAdd(transaction.Id, transaction))
            {
                return new ErrorResponse<Transaction>(
                    $"Could not add the transaction: {transaction.Id} to the pending list", transaction);
            }

            if (_pendingTransactions.Count % _configuration.BlockSize != 0)
            {
                return new SuccessResponse<Transaction>("The transaction has been added to pending list",
                    transaction);
            }

            // Launches mining
            var enqueueTime = DateTime.UtcNow;
            _queue.QueueMiningTask(token => new Task(() =>
            {
                var transactions = _pendingTransactions.Select(t => t.Value).OrderByDescending(t => t.Fee)
                    .Take(_configuration.BlockSize).ToList();
                transactions.ForEach(t => _pendingTransactions.TryRemove(t.Id, out _));
                _miningService.MineBlocks(transactions, enqueueTime, token);
            }, token));

            return new SuccessResponse<Transaction>("The transaction has been added and processing has started",
                transaction);
        }

        public BaseResponse<List<Transaction>> GetPendingTransactions()
        {
            var result = _pendingTransactions.Select(t => t.Value).ToList().OrderByDescending(t => t.Fee).ToList();
            var message = $"Pending transactions count: {_pendingTransactions.Count}/{_configuration.BlockSize}";

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