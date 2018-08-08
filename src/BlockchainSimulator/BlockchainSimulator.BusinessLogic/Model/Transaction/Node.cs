using Newtonsoft.Json;

namespace BlockchainSimulator.BusinessLogic.Model.Transaction
{
    public class Node : MerkleNode
    {
        [JsonProperty("leftNode")] 
        public MerkleNode LeftNode { get; set; }

        [JsonProperty("rightNode")]
        public MerkleNode RightNode { get; set; }
    }
}