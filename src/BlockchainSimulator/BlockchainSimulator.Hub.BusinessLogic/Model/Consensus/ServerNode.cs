using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace BlockchainSimulator.Hub.BusinessLogic.Model.Consensus
{
    /// <summary>
    /// The other node of the network
    /// </summary>
    public class ServerNode
    {
        /// <summary>
        /// Collection of id that the node is connected to
        /// </summary>
        [JsonProperty("connectedTo", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ConnectedTo { get; set; }

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
        /// The thread of the node
        /// </summary>
        [JsonIgnore]
        public Process NodeThread { get; set; }

        /// <summary>
        /// The hub connection
        /// </summary>
        [JsonIgnore]
        public HubConnection HubConnection { get; set; }

        /// <summary>
        /// Represents status of the node
        /// </summary>
        [JsonIgnore]
        public bool IsWorking { get; set; } = true;
    }
}