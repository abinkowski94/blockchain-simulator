using Newtonsoft.Json;

namespace BlockchainSimulator.BusinessLogic.Model.Transaction
{
    public class Leaf : MerkleNode
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; set; }

        [JsonIgnore]
        public Transaction Transaction { get; set; }
    }
}