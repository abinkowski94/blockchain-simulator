using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Transaction
{
    public class Node : MerkleNode
    {
        [JsonProperty("leftNode")]
        public Node LeftNode { get; }
        
        [JsonProperty("rightNode")]
        public Node RightNode { get; }
        
        public Node(string hash, Node leftNode, Node rightNode) : base(hash)
        {
            LeftNode = leftNode;
            RightNode = rightNode;
        }
    }
}