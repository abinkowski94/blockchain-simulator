using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class BlockchainStatistics
    {
        public int BlocksCount { get; set; }
        
        public TimeSpan TotalQueueTimeForBlocks { get; set; }
        
        public int TotalTransactionsCount { get; set; }
        
        public List<TransactionStatistics> TransactionsStatistics { get; set; }
    }
}