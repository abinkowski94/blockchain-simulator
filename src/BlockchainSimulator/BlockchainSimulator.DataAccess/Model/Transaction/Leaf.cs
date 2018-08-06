using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Transaction
{
    public class Leaf : MerkleNode
    {
        [JsonProperty("transactionId")]
        public string TransactionId { get; }
        
        public Leaf(string hash, string transactionId) : base(hash)
        {
            TransactionId = transactionId;
        }
    }
}