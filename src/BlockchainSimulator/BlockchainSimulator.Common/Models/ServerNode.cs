using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainSimulator.Common.Models
{
    /// <summary>
    /// The other node of the network
    /// </summary>
    public class ServerNode
    {
        /// <summary>
        /// Collection of id that the node is connected to
        /// </summary>
        [JsonProperty("connectedTo", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
        public IEnumerable<string> ConnectedTo { get; set; }

        /// <summary>
        /// Delay in connection
        /// </summary>
        [JsonProperty("delay", Order = 3)]
        public long Delay { get; set; }

        /// <summary>
        /// The HTTP address
        /// </summary>
        [JsonProperty("httpAddress", Order = 2)]
        public string HttpAddress { get; set; }

        /// <summary>
        /// Id of the node
        /// </summary>
        [JsonProperty("id", Order = 1)]
        public string Id { get; set; }

        /// <summary>
        /// Is the node responding (connected)
        /// </summary>
        [JsonProperty("isConnected", Order = 4)]
        public bool? IsConnected { get; set; }

        /// <summary>
        /// Indicates if the server needs to be spawned
        /// </summary>
        [JsonProperty("needsSpawn", Order = 6, NullValueHandling = NullValueHandling.Ignore)]
        public bool NeedsSpawn { get; set; }
    }
}