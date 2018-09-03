using BlockchainSimulator.Hub.BusinessLogic.Model.Consensus;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios
{
    /// <summary>
    /// The simulation
    /// </summary>
    public class Simulation
    {
        /// <summary>
        /// The configuration of the blockchain
        /// </summary>
        [JsonProperty("blockchainConfiguration")]
        public BlockchainConfiguration BlockchainConfiguration { get; set; }

        /// <summary>
        /// The date and time of last run
        /// </summary>
        [JsonProperty("lastRunTime")]
        public DateTime? LastRunTime { get; set; }

        /// <summary>
        /// The id of scenario
        /// </summary>
        [JsonProperty("scenarioId")]
        public Guid ScenarioId { get; set; }

        /// <summary>
        /// The server nodes
        /// </summary>
        [JsonProperty("serverNodes")]
        public List<ServerNode> ServerNodes { get; set; }

        /// <summary>
        /// The current status of simulation
        /// </summary>
        [JsonIgnore]
        public SimulationStatuses Status { get; set; }
    }
}