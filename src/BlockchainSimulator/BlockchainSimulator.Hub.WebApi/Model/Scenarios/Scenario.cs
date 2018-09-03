using Newtonsoft.Json;
using System;

namespace BlockchainSimulator.Hub.WebApi.Model.Scenarios
{
    /// <summary>
    /// The test scenario
    /// </summary>
    public class Scenario
    {
        /// <summary>
        /// The date of creation
        /// </summary>
        [JsonProperty("createDate", Order = 3)]
        public DateTime CreateDate { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        [JsonProperty("id", Order = 1)]
        public Guid Id { get; set; }

        /// <summary>
        /// The date of modification
        /// </summary>
        [JsonProperty("modificationDate", Order = 4)]
        public DateTime? ModificationDate { get; set; }

        /// <summary>
        /// Name of the scenario
        /// </summary>
        [JsonProperty("name", Order = 2)]
        public string Name { get; set; }

        /// <summary>
        /// The simulation
        /// </summary>
        [JsonProperty("simulation", Order = 5, NullValueHandling = NullValueHandling.Ignore)]
        public Simulation Simulation { get; set; }
    }
}