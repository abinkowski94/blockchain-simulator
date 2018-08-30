using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public abstract class MerkleNode
    {
        [JsonProperty("hash", Order = 1)]
        public string Hash { get; set; }
    }
}