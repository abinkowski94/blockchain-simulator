namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class Statistic
    {
        public int BlockSize { get; set; }

        public string Target { get; set; }

        public string Version { get; set; }

        public string NodeType { get; set; }

        public BlockchainStatistics BlockchainStatistics { get; set; }

        public MiningQueueStatistics MiningQueueStatistics { get; set; }
    }
}