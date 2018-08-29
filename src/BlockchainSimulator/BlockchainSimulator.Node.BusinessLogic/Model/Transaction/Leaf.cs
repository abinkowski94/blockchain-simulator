using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
{
    public class Leaf : MerkleNode
    {
        [JsonIgnore]
        public Transaction Transaction { get; set; }

        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }
    }
}