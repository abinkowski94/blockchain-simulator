using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The statistics session
    /// </summary>
    public class Statistic
    {
        /// <summary>
        /// The block size
        /// </summary>
        [JsonProperty("blockSize", Order = 1)]
        public int BlockSize { get; set; }

        /// <summary>
        /// The target of block
        /// </summary>
        [JsonProperty("target", Order = 2)]
        public string Target { get; set; }

        /// <summary>
        /// The version of protocol
        /// </summary>
        [JsonProperty("version", Order = 3)]
        public string Version { get; set; }

        /// <summary>
        /// The node type
        /// </summary>
        [JsonProperty("nodeType", Order = 4)]
        public string NodeType { get; set; }

        /// <summary>
        /// The statistics of the mining queue
        /// </summary>
        [JsonProperty("miningQueueStatistics", Order = 5)]
        public MiningQueueStatistics MiningQueueStatistics { get; set; }

        /// <summary>
        /// The communication queue statistics
        /// </summary>
        [JsonProperty("communicationQueueStatistics", Order = 6)]
        public CommunicationQueueStatistics CommunicationQueueStatistics { get; set; }

        /// <summary>
        /// The statistics of the blockchain
        /// </summary>
        [JsonProperty("blockchainStatistics", Order = 7)]
        public BlockchainStatistics BlockchainStatistics { get; set; }
    }
}