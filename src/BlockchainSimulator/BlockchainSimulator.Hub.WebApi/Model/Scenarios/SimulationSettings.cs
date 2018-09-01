using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.WebApi.Model.Scenarios
{
    /// <summary>
    /// The simulation settings
    /// </summary>
    public class SimulationSettings
    {
        /// <summary>
        /// Ends the simulation after given time
        /// </summary>
        [JsonProperty("forceEndAfter")]
        public TimeSpan? ForceEndAfter { get; set; }

        /// <summary>
        /// Dictionary of nodes and number of transactions to be sent
        /// </summary>
        [JsonProperty("nodesAndTransactions")]
        public Dictionary<string, long> NodesAndTransactions { get; set; }
    }
}