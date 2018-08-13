using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.BusinessLogic.Configurations;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly object _padlock = new object();
        private readonly ConcurrentDictionary<string, Transaction> _pendingTransactions;
        private readonly IBlockchainService _blockchainService;
        private readonly IMiningService _miningService;

        public TransactionService(IBlockchainService blockchainService, IMiningService miningService)
        {
            _pendingTransactions = new ConcurrentDictionary<string, Transaction>();
            _blockchainService = blockchainService;
            _miningService = miningService;
        }

        public Transaction AddTransaction(Transaction transaction)
        {
            transaction.Id = Guid.NewGuid().ToString();
            transaction.TransactionDetails = null;

            _pendingTransactions.TryAdd(transaction.Id, transaction);

            // Launches mining
            // TODO: adjust configuration
            if (ProofOfWorkConfigurations.BlockSize < _pendingTransactions.Count)
            {
                lock (_padlock)
                {
                    var transactions = _pendingTransactions.ToList().OrderByDescending(t => t.Value.Fee)
                        .Select(t => t.Value).ToList();
                    transactions.ForEach(t => _pendingTransactions.TryRemove(t.Id, out _));
                    _miningService.MineBlocks(transactions);
                }
            }

            return transaction;
        }

        public List<Transaction> GetPendingTransactions()
        {
            return _pendingTransactions.ToList().OrderByDescending(t => t.Value.Fee).Select(t => t.Value).ToList();
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