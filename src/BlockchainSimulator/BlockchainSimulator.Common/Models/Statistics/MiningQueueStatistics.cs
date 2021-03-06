using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Common.Models.Statistics
{
    /// <summary>
    /// The mining queue statistics
    /// </summary>
    public class MiningQueueStatistics
    {
        /// <summary>
        /// The number of abandoned blocks work
        /// </summary>
        [JsonProperty("abandonedBlocksCount", Order = 2)]
        public int AbandonedBlocksCount { get; set; }

        /// <summary>
        /// Average queue time
        /// </summary>
        [JsonProperty("averageQueueTime", Order = 6)]
        public TimeSpan AverageQueueTime { get; set; }

        /// <summary>
        /// Current queue length
        /// </summary>
        [JsonProperty("currentQueueLength", Order = 4)]
        public int CurrentQueueLength { get; set; }

        /// <summary>
        /// The max queue length
        /// </summary>
        [JsonProperty("maxQueueLength", Order = 3)]
        public int MaxQueueLength { get; set; }

        /// <summary>
        /// The number of rejected incoming blockchains
        /// </summary>
        [JsonProperty("rejectedIncomingBlockchainCount", Order = 7)]
        public int RejectedIncomingBlockchainCount { get; set; }

        /// <summary>
        /// The number of mining attempts
        /// </summary>
        [JsonProperty("totalMiningAttemptsCount", Order = 1)]
        public int TotalMiningAttemptsCount { get; set; }

        /// <summary>
        /// Total queue time
        /// </summary>
        [JsonProperty("totalQueueTime", Order = 5)]
        public TimeSpan TotalQueueTime { get; set; }
    }
}