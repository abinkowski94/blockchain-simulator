using System;
using System.Collections.Generic;
using BlockchainSimulator.Common.Models;
using Newtonsoft.Json;

namespace BlockchainSimulator.Hub.WebApi.Model
{
    /// <summary>
    /// The simulation
    /// </summary>
    public class Simulation
    {
        /// <summary>
        /// The id of scenario
        /// </summary>
        [JsonProperty("scenarioId")]
        public Guid ScenarioId { get; set; }
        
        /// <summary>
        /// The server nodes
        /// </summary>
        [JsonProperty("serverNodes")]
        public IEnumerable<ServerNode> ServerNodes { get; set; }
        
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
        /// The current status of simulation
        /// </summary>
        [JsonProperty("status")]
        public SimulationStatuses Status { get; set; }
    }
}