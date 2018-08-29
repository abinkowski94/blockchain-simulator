using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
{
    public class Node : MerkleNode
    {
        [JsonProperty("leftNode")]
        public MerkleNode LeftNode { get; set; }

        [JsonProperty("rightNode")]
        public MerkleNode RightNode { get; set; }
    }
}