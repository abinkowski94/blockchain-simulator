using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class MiningQueueStatistics
    {
        public int MaxQueueLength { get; set; }

        public int CurrentQueueLength { get; set; }
        
        public TimeSpan TotalQueueTime { get; set; }
        
        public TimeSpan AverageQueueTime { get; set; }
    }
}