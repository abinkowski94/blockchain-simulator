using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The blockchain node configuration
    /// </summary>
    public class BlockchainNodeConfiguration
    {
        /// <summary>
        /// The node Id
        /// </summary>
        [JsonProperty("nodeId")]
        public string NodeId { get; set; }

        /// <summary>
        /// The version of the node
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }

        /// <summary>
        /// The target of the node
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }

        /// <summary>
        /// The size of the block
        /// </summary>
        [JsonProperty("blockSize")]
        public int BlockSize { get; set; }

        /// <summary>
        /// The list of startup validators ids
        /// </summary>
        [JsonProperty("startupValidators")]
        public Dictionary<string, int> StartupValidatorsWithStakes { get; set; }

        /// <summary>
        /// The type of the consensus algorithm
        /// </summary>
        [JsonProperty("nodeType")]
        public string NodeType { get; set; }

        /// <summary>
        /// Determinate weather node is a validator
        /// </summary>
        [JsonProperty("nodeIsValidator")]
        public bool NodeIsValidator { get; set; }

        /// <summary>
        /// The size of the epoch
        /// </summary>
        [JsonProperty("epochSize")]
        public int EpochSize { get; set; }
    }
}