using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
{
    public abstract class MerkleNode
    {
        [JsonProperty("hash")] 
        public string Hash { get; set; }
    }
}