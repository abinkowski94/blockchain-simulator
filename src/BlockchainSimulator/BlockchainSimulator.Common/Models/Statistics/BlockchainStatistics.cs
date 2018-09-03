using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models.Statistics
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

        /// <summary>
        /// The blockchain branches
        /// </summary>
        [JsonProperty("blockchainBranches", Order = 4)]
        public List<List<BlockInfo>> BlockchainBranches { get; set; }

        /// <summary>
        /// The transactions statistics
        /// </summary>
        [JsonProperty("transactionsStatistics", Order = 5)]
        public List<TransactionStatistics> TransactionsStatistics { get; set; }
    }
}