using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The blockchain statistics
    /// </summary>
    public class BlockchainStatistics
    {
        /// <summary>
        /// The number of blocks in blockchain
        /// </summary>
        [JsonProperty("blocksCount", Order = 1)]
        public int BlocksCount { get; set; }

        /// <summary>
        /// The total transactions count
        /// </summary>
        [JsonProperty("totalTransactionsCount", Order = 2)]
        public int TotalTransactionsCount { get; set; }

        /// <summary>
        /// The total queue time for all blocks
        /// </summary>
        [JsonProperty("totalQueueTimeForBlocks", Order = 3)]
        public TimeSpan TotalQueueTimeForBlocks { get; set; }
    }
}