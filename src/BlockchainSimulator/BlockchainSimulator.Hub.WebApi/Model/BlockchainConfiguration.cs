using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.WebApi.Model
{
    /// <summary>
    /// The settings of the simulation
    /// </summary>
    public class BlockchainConfiguration
    {
        /// <summary>
        /// Version of the protocol
        /// </summary>
        [JsonProperty("version")]
        public string Version { get; set; }
        
        /// <summary>
        /// Target of proof of work
        /// </summary>
        [JsonProperty("target")]
        public string Target { get; set; }
        
        /// <summary>
        /// The block size
        /// </summary>
        [JsonProperty("blockSize")]
        public string BlockSize { get; set; }
    }
}