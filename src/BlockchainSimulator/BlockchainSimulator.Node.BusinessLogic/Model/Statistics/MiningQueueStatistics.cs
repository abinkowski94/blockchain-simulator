using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class MiningQueueStatistics
    {
        public int AbandonedBlocksCount { get; set; }
        public TimeSpan AverageQueueTime { get; set; }
        public int CurrentQueueLength { get; set; }
        public int MaxQueueLength { get; set; }
        public int RejectedIncomingBlockchainCount { get; set; }
        public int TotalMiningAttemptsCount { get; set; }
        public TimeSpan TotalQueueTime { get; set; }
    }
}