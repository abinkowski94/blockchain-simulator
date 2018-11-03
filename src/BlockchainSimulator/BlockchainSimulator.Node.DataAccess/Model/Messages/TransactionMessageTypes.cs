namespace BlockchainSimulator.Node.DataAccess.Model.Messages
{
    public enum TransactionMessageTypes
    {
        None = 0,
        Prepare = 1,
        Commit = 2,
        Deposit = 3,
        Withdraw = 4
    }
}