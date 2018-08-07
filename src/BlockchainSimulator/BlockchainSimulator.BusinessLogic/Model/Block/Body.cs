using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Model.Block
{
    public class Body
    {
        public Node MerkleTree { get; set; }
        
        public HashSet<Transaction.Transaction> Transactions { get; set; }
        
        public int TransactionCounter => Transactions.Count;
    }
}