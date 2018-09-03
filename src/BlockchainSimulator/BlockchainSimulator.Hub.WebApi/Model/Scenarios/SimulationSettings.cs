using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
        [JsonProperty("forceEndAfter", Order = 2)]
        public TimeSpan? ForceEndAfter { get; set; }

        /// <summary>
        /// Dictionary of nodes and number of transactions to be sent
        /// </summary>
        [JsonProperty("nodesAndTransactions", Order = 1)]
        public Dictionary<string, long> NodesAndTransactions { get; set; }
    }
}