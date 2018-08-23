using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models
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
        
        /// <summary>
        /// Indicates if the server needs to be spawned
        /// </summary>
        [JsonProperty("needsSpawn", NullValueHandling = NullValueHandling.Ignore)]
        public bool NeedsSpawn { get; set; }
        
        /// <summary>
        /// Collection of id that the node is connected to
        /// </summary>
        [JsonProperty("connectedTo", NullValueHandling =  NullValueHandling.Ignore)]
        public IEnumerable<string> ConnectedTo { get; set; }
    }
}