using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model.Transaction
{
    public class Node : MerkleNode
    {
        [JsonProperty("leftNode", Order = 2)]
        public MerkleNode LeftNode { get; set; }

        [JsonProperty("rightNode", Order = 3)]
        public MerkleNode RightNode { get; set; }
    }
}