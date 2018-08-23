using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public class Leaf : MerkleNode
    {
        [JsonProperty("transactionId")] public string TransactionId { get; set; }
    }
}