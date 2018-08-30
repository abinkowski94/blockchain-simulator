using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The status of the mining queue
    /// </summary>
    public class MiningQueueStatus
    {
        /// <summary>
        /// Is queue empty
        /// </summary>
        [JsonProperty("isEmpty", Order = 1)]
        public bool IsEmpty => Length == 0;

        /// <summary>
        /// The length of the queue
        /// </summary>
        [JsonProperty("length", Order = 2)]
        public int Length { get; set; }
    }
}