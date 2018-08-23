using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public abstract class MerkleNode
    {
        [JsonProperty("hash")] public string Hash { get; set; }
    }
}