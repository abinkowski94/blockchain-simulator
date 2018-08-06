using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Transaction
{
    public abstract class MerkleNode
    {
        [JsonProperty("hash")]
        public string Hash { get; }

        public MerkleNode(string hash)
        {
            Hash = hash;
        }
    }
}