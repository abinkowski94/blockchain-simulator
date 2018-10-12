using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Storage;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class TransactionService : BaseService, ITransactionService
    {
        private readonly ITransactionStorage _transactionStorage;
        private readonly IBlockchainService _blockchainService;
        private readonly IMiningService _miningService;
        private readonly IMiningQueue _queue;

        public TransactionService(IConfigurationService configurationService, IBlockchainService blockchainService,
            IMiningService miningService, IMiningQueue queue, ITransactionStorage transactionStorage)
            : base(configurationService)
        {
            _transactionStorage = transactionStorage;
            _blockchainService = blockchainService;
            _miningService = miningService;
            _queue = queue;
        }

        public BaseResponse<Transaction> AddTransaction(Transaction transaction)
        {
            transaction.Id = transaction.Id ?? $"{Guid.NewGuid().ToString()}-{BlockchainNodeConfiguration.NodeId}";
            transaction.RegistrationTime = transaction.RegistrationTime ?? DateTime.UtcNow;
            transaction.TransactionDetails = null;

            _transactionStorage.RegisteredTransactions.TryAdd(transaction.Id, transaction);
            if (!_transactionStorage.PendingTransactions.TryAdd(transaction.Id, transaction))
            {
                return new ErrorResponse<Transaction>(
                    $"Could not add the transaction: {transaction.Id} to the pending list", transaction);
            }

            if (_transactionStorage.PendingTransactions.Count % BlockchainNodeConfiguration.BlockSize != 0)
            {
                return new SuccessResponse<Transaction>("The transaction has been added to pending list",
                    transaction);
            }

            MineTransactions();
            return new SuccessResponse<Transaction>("The transaction has been added and processing has started",
                transaction);
        }

        public BaseResponse<List<Transaction>> AddTransactions(IEnumerable<Transaction> transactions)
        {
            var responses = transactions.Select(AddTransaction).ToList();
            var result = responses.Select(r => r.Result).ToList();

            return new SuccessResponse<List<Transaction>>("The transactions has been added!", result);
        }

        public BaseResponse<bool> ForceMining()
        {
            MineTransactions();
            return new SuccessResponse<bool>("Mining has been forced successfully!", true);
        }

        public BaseResponse<List<Transaction>> GetPendingTransactions()
        {
            var result = _transactionStorage.PendingTransactions.Values.OrderByDescending(t => t.Fee).ToList();
            var count = $"{_transactionStorage.PendingTransactions.Count}/{BlockchainNodeConfiguration.BlockSize}";
            return new SuccessResponse<List<Transaction>>($"Pending transactions count: {count}", result);
        }

        public BaseResponse<Transaction> GetTransaction(string id)
        {
            var blockchainResponse = _blockchainService.GetBlockchainTree();
            if (!blockchainResponse.IsSuccess)
            {
                return new ErrorResponse<Transaction>("An error occurred while reading blockchain from local storage!",
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

        private void MineTransactions()
        {
            var enqueueTime = DateTime.UtcNow;
            _queue.EnqueueTask(token => new Task(() =>
            {
                var transactions = _transactionStorage.PendingTransactions.Values.OrderByDescending(t => t.Fee)
                    .Take(BlockchainNodeConfiguration.BlockSize).ToList();
                transactions.ForEach(t => _transactionStorage.PendingTransactions.TryRemove(t.Id, out _));
                _miningService.MineBlock(transactions.ToHashSet(), enqueueTime, token);
            }, token));
        }
    }
}