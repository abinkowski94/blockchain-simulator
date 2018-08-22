using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Transaction
{
    public class Node : MerkleNode
    {
        [JsonProperty("leftNode")] public MerkleNode LeftNode { get; set; }

        [JsonProperty("rightNode")] public MerkleNode RightNode { get; set; }
    }
}