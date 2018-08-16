using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models
{
    /// <summary>
    /// The other node of the network
    /// </summary>
    public class ServerNode
    {
        /// <summary>
        /// Delay in connection
        /// </summary>
        [JsonProperty("delay")]
        public long Delay { get; set; }

        /// <summary>
        /// The HTTP address
        /// </summary>
        [JsonProperty("httpAddress")]
        public string HttpAddress { get; set; }

        /// <summary>
        /// Id of the node
        /// </summary>
        [JsonProperty("id")]
        public string Id { get; set; }

        /// <summary>
        /// Is the node responding (connected)
        /// </summary>
        [JsonProperty("isConnected")]
        public bool? IsConnected { get; set; }
    }
}