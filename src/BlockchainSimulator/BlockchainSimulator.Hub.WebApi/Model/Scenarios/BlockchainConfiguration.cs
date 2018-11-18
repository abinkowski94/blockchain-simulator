using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.WebApi.Model.Scenarios
{
    /// <summary>
    /// The settings of the simulation
    /// </summary>
    public class BlockchainConfiguration
    {
        /// <summary>
        /// The block size
        /// </summary>
        [JsonProperty("blockSize", Order = 3)]
        public string BlockSize { get; set; }

        /// <summary>
        /// Target of proof of work
        /// </summary>
        [JsonProperty("target", Order = 4)]
        public string Target { get; set; }

        /// <summary>
        /// The type of consensus algorithm
        /// </summary>
        [JsonProperty("type", Order = 1)]
        public string Type { get; set; }

        /// <summary>
        /// Version of the protocol
        /// </summary>
        [JsonProperty("version", Order = 2)]
        public string Version { get; set; }
        
        /// <summary>
        /// The epoch size
        /// </summary>
        [JsonProperty("epochSize")]
        public int EpochSize { get; set; }
    }
}