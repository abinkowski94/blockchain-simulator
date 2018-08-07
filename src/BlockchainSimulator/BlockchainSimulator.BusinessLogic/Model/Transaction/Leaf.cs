namespace BlockchainSimulator.BusinessLogic.Model.Transaction
{
    public class Leaf : MerkleNode
    {
        public Transaction Transaction { get; set; }
        
        public string TransactionId { get; set; }
    }
}