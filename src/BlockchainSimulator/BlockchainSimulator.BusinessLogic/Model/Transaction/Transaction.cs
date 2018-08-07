namespace BlockchainSimulator.BusinessLogic.Model.Transaction
{
    public class Transaction
    {
        public string Id { get; set; }
        
        public string Sender { get; set; }
        
        public string Recipient { get; set; }
        
        public decimal Amount { get; set; }
        
        public decimal Fee { get; set; }
        
        public string TransactionJson { get; set; }
    }
}