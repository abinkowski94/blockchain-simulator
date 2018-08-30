using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public class Leaf : MerkleNode
    {
        [JsonProperty("transactionId", Order = 2)]
        public string TransactionId { get; set; }
    }
}