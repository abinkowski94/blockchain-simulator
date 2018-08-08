using Newtonsoft.Json;

namespace BlockchainSimulator.BusinessLogic.Model.Transaction
{
    public abstract class MerkleNode
    {
        [JsonProperty("hash")] 
        public string Hash { get; set; }
    }
}