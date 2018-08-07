namespace BlockchainSimulator.BusinessLogic.Model.Transaction
{
    public class Node : MerkleNode
    {
        public MerkleNode LeftNode { get; set; }

        public MerkleNode RightNode { get; set; }
    }
}