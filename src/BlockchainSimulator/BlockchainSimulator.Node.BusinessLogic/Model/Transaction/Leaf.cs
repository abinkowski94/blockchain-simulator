using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
{
    public class Leaf : MerkleNode
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonIgnore]
        public Transaction Transaction { get; set; }
    }
}