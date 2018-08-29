namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
{
    public class TransactionDetails
    {
        public string BlockId { get; set; }
        public long BlocksBehind { get; set; }
        public bool IsConfirmed { get; set; }
    }
}