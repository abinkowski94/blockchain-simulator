using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class TransactionStatistics
    {
        public string BlockId { get; set; }

        public TimeSpan BlockQueueTime { get; set; }

        public TimeSpan TransactionConfirmationTime { get; set; }
        public decimal TransactionFee { get; set; }
        public string TransactionId { get; set; }
        public DateTime TransactionRegistrationTime { get; set; }
    }
}