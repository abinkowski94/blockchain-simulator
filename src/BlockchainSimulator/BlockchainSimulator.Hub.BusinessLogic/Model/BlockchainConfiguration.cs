using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.BusinessLogic.Model
{
    /// <summary>
    /// The settings of the simulation
    /// </summary>
    public class BlockchainConfiguration
    {
        /// <summary>
        /// The block size
        /// </summary>
        [JsonProperty("blockSize")]
        public string BlockSize { get; set; }

        /// <summary>
        /// Target of proof of work
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }

        /// <summary>
        /// The type of consensus algorithm
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// Version of the protocol
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
    }
}