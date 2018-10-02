using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models.Statistics
{
    /// <summary>
    /// The status of the mining queue
    /// </summary>
    public class MiningQueueStatus
    {
        /// <summary>
        /// Is queue empty
        /// </summary>
        [JsonProperty("isWorking", Order = 1)]
        public bool IsWorking { get; set; }

        /// <summary>
        /// The length of the queue
        /// </summary>
        [JsonProperty("length", Order = 2)]
        public int Length { get; set; }
    }
}