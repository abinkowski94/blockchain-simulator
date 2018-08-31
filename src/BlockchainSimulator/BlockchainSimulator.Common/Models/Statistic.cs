using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The statistics session
    /// </summary>
    public class Statistic
    {
        /// <summary>
        /// The statistics of the blockchain
        /// </summary>
        [JsonProperty("blockchainStatistics", Order = 1)]
        public BlockchainStatistics BlockchainStatistics { get; set; }

        /// <summary>
        /// The statistics of the mining queue
        /// </summary>
        [JsonProperty("miningQueueStatistics", Order = 2)]
        public MiningQueueStatistics MiningQueueStatistics { get; set; }
    }
}