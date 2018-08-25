using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Hub.BusinessLogic.Model
{
    /// <summary>
    /// The test scenario
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// The date of creation
        /// </summary>
        [JsonProperty("createDate")]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// The date of modification
        /// </summary>
        [JsonProperty("modificationDate")]
        public DateTime ModificationDate { get; set; }

        /// <summary>
        /// Name of the scenario
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// The simulation
        /// </summary>
        [JsonProperty("simulation", NullValueHandling = NullValueHandling.Ignore)]
        public Simulation Simulation { get; set; }
    }
}