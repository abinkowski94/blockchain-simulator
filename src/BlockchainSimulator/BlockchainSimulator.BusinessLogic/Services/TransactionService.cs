using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ConcurrentBag<Transaction> _pendingTransactions;
        private readonly IBlockchainService _blockchainService;

        public TransactionService(IBlockchainService blockchainService)
        {
            _pendingTransactions = new ConcurrentBag<Transaction>();
            _blockchainService = blockchainService;
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid().ToString();
            transaction.TransactionDetails = null;

            _pendingTransactions.Add(transaction);
            
            return transaction;
        }

        public List<Transaction> GetPendingTransactions()
        {
            return _pendingTransactions.ToList().OrderByDescending(t => t.Fee).ToList();
        }

        public Transaction GetTransaction(string id)
        {
            var blockchain = _blockchainService.GetBlockchain();
            return FindAndFillTransactionData(id, blockchain);
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