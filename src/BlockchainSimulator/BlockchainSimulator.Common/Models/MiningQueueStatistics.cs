using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The mining queue statistics
    /// </summary>
    public class MiningQueueStatistics
    {
        /// <summary>
        /// The max queue length
        /// </summary>
        [JsonProperty("maxQueueLength", Order = 1)]
        public int MaxQueueLength { get; set; }

        /// <summary>
        /// Current queue length
        /// </summary>
        [JsonProperty("currentQueueLength", Order = 2)]
        public int CurrentQueueLength { get; set; }

        /// <summary>
        /// The number of mining attempts
        /// </summary>
        [JsonProperty("totalMiningAttemptsCount", Order = 3)]
        public int TotalMiningAttemptsCount { get; set; }

        /// <summary>
        /// The number of abandoned blocks work
        /// </summary>
        [JsonProperty("abandonedBlocksCount", Order = 4)]
        public int AbandonedBlocksCount { get; set; }

        /// <summary>
        /// Total queue time
        /// </summary>
        [JsonProperty("totalQueueTime", Order = 5)]
        public TimeSpan TotalQueueTime { get; set; }

        /// <summary>
        /// Average queue time
        /// </summary>
        [JsonProperty("averageQueueTime", Order = 6)]
        public TimeSpan AverageQueueTime { get; set; }
    }
}