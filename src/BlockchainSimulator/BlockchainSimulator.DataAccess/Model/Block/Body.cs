using System.Collections.Generic;
using BlockchainSimulator.DataAccess.Model.Transaction;
using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public class Body
    {
        [JsonProperty("merkleTree")] public Node MerkleTree { get; set; }

        [JsonProperty("transactions")] public HashSet<Transaction.Transaction> Transactions { get; set; }

        [JsonProperty("transactionCounter")] public int TransactionCounter => Transactions.Count;
    }
}