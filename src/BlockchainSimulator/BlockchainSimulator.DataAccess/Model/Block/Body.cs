using System.Collections.Generic;
using BlockchainSimulator.DataAccess.Model.Transaction;
using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public class Body
    {
        [JsonProperty("merkleTree")]
        public Node MerkleTree { get; }
        
        [JsonProperty("transactions")]
        public HashSet<Transaction.Transaction> Transactions { get; }
        
        [JsonProperty("transactionCounter")]
        public int TransactionCounter => Transactions.Count;

        public Body(Node merkleTree)
        {
            MerkleTree = merkleTree;
            Transactions = new HashSet<Transaction.Transaction>();
        }
    }
}