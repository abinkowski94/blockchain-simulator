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
        /// The id of the block
        /// </summary>
        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// The nonce value
        /// </summary>
        [JsonProperty("nonce", Order = 3)]
        public string Nonce { get; set; }

        /// <summary>
        /// The timestamp of the block
        /// </summary>
        [JsonProperty("timeStamp", Order = 2)]
        public DateTime TimeStamp { get; set; }
    }
}