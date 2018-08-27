using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Model
{
    /// <summary>
    /// The simulation settings
    /// </summary>
    public class SimulationSettings
    {
        /// <summary>
        /// Ends the simulation after given time
        /// </summary>
        public TimeSpan? ForceEndAfter { get; set; }

        /// <summary>
        /// Dictionary of nodes and number of transactions to be sent
        /// </summary>
        public Dictionary<string, long> NodesAndTransactions { get; set; }
    }
}