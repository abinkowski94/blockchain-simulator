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
        /// Dictionary of nodes and number of transactions to be sent
        /// </summary>
        [JsonProperty("nodesAndTransactions", Order = 1)]
        public Dictionary<string, int> NodesAndTransactions { get; set; }

        /// <summary>
        /// Sends all transactions in one request
        /// </summary>
        [JsonProperty("sendTransactionsTogether", Order = 2)]
        public bool SendTransactionsTogether { get; set; }

        /// <summary>
        /// Ends the simulation after given time
        /// </summary>
        [JsonProperty("forceEndAfter", Order = 3)]
        public TimeSpan? ForceEndAfter { get; set; }
    }
}