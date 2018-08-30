using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.DataAccess.Model.Block
{
    public class Body
    {
        [JsonProperty("merkleTree", Order = 3)]
        public Transaction.Node MerkleTree { get; set; }

        [JsonProperty("transactionCounter", Order = 1)]
        public int TransactionCounter => Transactions.Count;
        
        [JsonProperty("transactions", Order = 2)]
        public HashSet<Transaction.Transaction> Transactions { get; set; }
    }
}