using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class Body
    {
        [JsonProperty("merkleTree")]
        public Transaction.Node MerkleTree { get; set; }
        
        [JsonProperty("transactions")]
        public HashSet<Transaction.Transaction> Transactions { get; set; }
        
        [JsonProperty("transactionCounter")]
        public int TransactionCounter => Transactions.Count;
    }
}