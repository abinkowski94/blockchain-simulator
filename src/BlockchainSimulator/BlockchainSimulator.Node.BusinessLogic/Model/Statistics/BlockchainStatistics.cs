using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class BlockchainStatistics
    {
        public int BlocksCount { get; set; }
        
        public TimeSpan TotalQueueTimeForBlocks { get; set; }
    }
}