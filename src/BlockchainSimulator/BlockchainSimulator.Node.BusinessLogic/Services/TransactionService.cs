using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ConcurrentDictionary<string, Transaction> _pendingTransactions;
        private readonly IConfigurationService _configurationService;
        private readonly IBlockchainService _blockchainService;
        private readonly IMiningService _miningService;
        private readonly IMiningQueue _queue;
        private readonly object _padlock = new object();

        public ConcurrentDictionary<string, Transaction> RegisteredTransactions { get; }

        private BlockchainNodeConfiguration BlockchainNodeConfiguration => _configurationService.GetConfiguration();

        public TransactionService(IBlockchainService blockchainService, IMiningService miningService,
            IMiningQueue queue, IConfigurationService configurationService)
        {
            _pendingTransactions = new ConcurrentDictionary<string, Transaction>();
            _configurationService = configurationService;
            _blockchainService = blockchainService;
            _miningService = miningService;
            _queue = queue;

            RegisteredTransactions = new ConcurrentDictionary<string, Transaction>();
        }

        public BaseResponse<Transaction> AddTransaction(Transaction transaction)
        {
            lock (_padlock)
            {
                transaction.Id = transaction.Id ?? $"{Guid.NewGuid().ToString()}-{BlockchainNodeConfiguration.NodeId}";
                transaction.RegistrationTime = transaction.RegistrationTime ?? DateTime.UtcNow;
                transaction.TransactionDetails = null;

                RegisteredTransactions.TryAdd(transaction.Id, transaction);
                if (!_pendingTransactions.TryAdd(transaction.Id, transaction))
                {
                    return new ErrorResponse<Transaction>(
                        $"Could not add the transaction: {transaction.Id} to the pending list", transaction);
                }

                if (_pendingTransactions.Count % BlockchainNodeConfiguration.BlockSize != 0)
                {
                    return new SuccessResponse<Transaction>("The transaction has been added to pending list",
                        transaction);
                }

                MineTransactions();
                return new SuccessResponse<Transaction>("The transaction has been added and processing has started",
                    transaction);
            }
        }

        public BaseResponse<List<Transaction>> AddTransactions(List<Transaction> transactions)
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
            var result = _pendingTransactions.Values.OrderByDescending(t => t.Fee).ToList();
            var message =
                $"Pending transactions count: {_pendingTransactions.Count}/{BlockchainNodeConfiguration.BlockSize}";

            return new SuccessResponse<List<Transaction>>(message, result);
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

        public void Clear()
        {
            _pendingTransactions.Clear();
            RegisteredTransactions.Clear();
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
                var transactions = _pendingTransactions.Values.OrderByDescending(t => t.Fee)
                    .Take(BlockchainNodeConfiguration.BlockSize).ToList();
                transactions.ForEach(t => _pendingTransactions.TryRemove(t.Id, out _));
                _miningService.MineBlock(transactions.ToHashSet(), enqueueTime, token);
            }, token));
        }
    }
}