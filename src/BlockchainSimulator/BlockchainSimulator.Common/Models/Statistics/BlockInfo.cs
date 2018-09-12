using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Common.Models.Statistics
{
    /// <summary>
    /// The block info
    /// </summary>
    public class BlockInfo
    {
        /// <summary>
        /// The unique id
        /// </summary>
        [JsonProperty("uniqueId", Order = 1)]
        public string UniqueId { get; set; }

        /// <summary>
        /// The parent unique id
        /// </summary>
        [JsonProperty("parentUniqueId", Order = 2)]
        public string ParentUniqueId { get; set; }

        /// <summary>
        /// The id of the block
        /// </summary>
        [JsonProperty("id", Order = 3)]
        public string Id { get; set; }

        /// <summary>
        /// The nonce value
        /// </summary>
        [JsonProperty("nonce", Order = 4)]
        public string Nonce { get; set; }

        /// <summary>
        /// The timestamp of the block
        /// </summary>
        [JsonProperty("timeStamp", Order = 5)]
        public DateTime TimeStamp { get; set; }
    }
}