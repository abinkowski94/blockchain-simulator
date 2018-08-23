namespace BlockchainSimulator.Node.BusinessLogic.Model.Transaction
{
    public class TransactionDetails
    {
        public bool IsConfirmed { get; set; }

        public string BlockId { get; set; }

        public long BlocksBehind { get; set; }
    }
}