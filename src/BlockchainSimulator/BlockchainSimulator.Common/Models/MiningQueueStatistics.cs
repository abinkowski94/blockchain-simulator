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
        /// Total queue time
        /// </summary>
        [JsonProperty("totalQueueTime", Order = 3)]
        public TimeSpan TotalQueueTime { get; set; }
        
        /// <summary>
        /// Average queue time
        /// </summary>
        [JsonProperty("averageQueueTime", Order = 4)]
        public TimeSpan AverageQueueTime { get; set; }
    }
}