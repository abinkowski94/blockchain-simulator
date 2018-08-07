using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Transaction
{
    public abstract class MerkleNode
    {
        [JsonProperty("hash")] 
        public string Hash { get; set; }
    }
}