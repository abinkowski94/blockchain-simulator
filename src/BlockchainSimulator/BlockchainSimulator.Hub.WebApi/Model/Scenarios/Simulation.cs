using BlockchainSimulator.Common.Models.Consensus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.WebApi.Model.Scenarios
{
    /// <summary>
    /// The simulation
    /// </summary>
    public class Simulation
    {
        /// <summary>
        /// The configuration of the blockchain
        /// </summary>
        [JsonProperty("blockchainConfiguration", Order = 4)]
        public BlockchainConfiguration BlockchainConfiguration { get; set; }

        /// <summary>
        /// The date and time of last run
        /// </summary>
        [JsonProperty("lastRunTime", Order = 3)]
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// The id of scenario
        /// </summary>
        [JsonProperty("scenarioId", Order = 1)]
        public Guid ScenarioId { get; set; }

        /// <summary>
        /// The server nodes
        /// </summary>
        [JsonProperty("serverNodes", Order = 5)]
        public List<ServerNode> ServerNodes { get; set; }

        /// <summary>
        /// The current status of simulation
        /// </summary>
        [JsonProperty("status", Order = 2)]
        public SimulationStatuses Status { get; set; }
    }
}