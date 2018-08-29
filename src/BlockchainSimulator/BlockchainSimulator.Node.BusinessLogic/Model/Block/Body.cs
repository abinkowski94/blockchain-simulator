using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Block
{
    public class Body
    {
        [JsonProperty("merkleTree")]
        public Transaction.Node MerkleTree { get; set; }

        [JsonProperty("transactionCounter")]
        public int TransactionCounter => Transactions.Count;

        [JsonProperty("transactions")]
        public HashSet<Transaction.Transaction> Transactions { get; set; }
    }
}