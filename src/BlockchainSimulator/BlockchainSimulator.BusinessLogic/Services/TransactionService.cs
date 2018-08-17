using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Configurations;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IBlockchainConfiguration _configuration;
        private readonly object _padlock = new object();
        private readonly ConcurrentDictionary<string, Transaction> _pendingTransactions;
        private readonly IBlockchainService _blockchainService;
        private readonly IMiningService _miningService;

        public TransactionService(IBlockchainService blockchainService, IMiningService miningService,
            IBlockchainConfiguration configuration)
        {
            _pendingTransactions = new ConcurrentDictionary<string, Transaction>();
            _blockchainService = blockchainService;
            _miningService = miningService;
            _configuration = configuration;
        }

        public BaseResponse<Transaction> AddTransaction(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid().ToString();
            transaction.TransactionDetails = null;

            if (!_pendingTransactions.TryAdd(transaction.Id, transaction))
            {
                return new ErrorResponse<Transaction>("Could not add the transaction to pending list!", null);
            }

            // Launches mining
            if (_configuration.BlockSize <= _pendingTransactions.Count)
            {
                lock (_padlock)
                {
                    var transactions = _pendingTransactions.ToList().OrderByDescending(t => t.Value.Fee)
                        .Select(t => t.Value).ToList();
                    transactions.ForEach(t => _pendingTransactions.TryRemove(t.Id, out _));
                    _miningService.MineBlocks(transactions);
                }

                return new SuccessResponse<Transaction>(
                    "The transaction has been added and processing has been started", transaction);
            }

            return new SuccessResponse<Transaction>("The transaction has been added to pending list", transaction);
        }

        public BaseResponse<List<Transaction>> GetPendingTransactions()
        {
            var result = _pendingTransactions.ToList().OrderByDescending(t => t.Value.Fee).Select(t => t.Value)
                .ToList();
            return new SuccessResponse<List<Transaction>>($"The list of pending transactions: {DateTime.UtcNow}",
                result);
        }

        public BaseResponse<Transaction> GetTransaction(string id)
        {
            var blockchainResponse = _blockchainService.GetBlockchain();
            if (blockchainResponse.IsSuccess)
            {
                var result = FindAndFillTransactionData(id, blockchainResponse.Result);
                if (result == null)
                {
                    return new ErrorResponse<Transaction>($"Could not find the transaction id: {id}", null);
                }

                return new SuccessResponse<Transaction>("The transaction has been found", result);
            }

            return new ErrorResponse<Transaction>("An error occured while reading blockchain from local storage!", null,
                blockchainResponse.Message);
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