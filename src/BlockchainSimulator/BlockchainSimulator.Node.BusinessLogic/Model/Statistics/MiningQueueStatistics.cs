using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class MiningQueueStatistics
    {
        public int MaxQueueLength { get; set; }

        public int CurrentQueueLength { get; set; }
        
        public TimeSpan TotalQueueTime { get; set; }
        
        public TimeSpan AverageQueueTime { get; set; }
        
        public int AbandonedBlocksCount { get; set; }
        
        public int TotalMiningAttemptsCount { get; set; }
        
        public int RejectedIncomingBlockchainCount { get; set; }
    }
}