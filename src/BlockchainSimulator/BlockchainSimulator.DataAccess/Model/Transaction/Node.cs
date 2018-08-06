using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Transaction
{
    public class Node : MerkleNode
    {
        [JsonProperty("leftNode")]
        public MerkleNode LeftNode { get; }
        
        [JsonProperty("rightNode")]
        public MerkleNode RightNode { get; }
        
        public Node(string hash, MerkleNode leftNode, MerkleNode rightNode) : base(hash)
        {
            LeftNode = leftNode;
            RightNode = rightNode;
        }
    }
}