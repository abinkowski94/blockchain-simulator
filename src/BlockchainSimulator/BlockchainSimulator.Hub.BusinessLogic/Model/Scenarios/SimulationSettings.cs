using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Model.Scenarios
{
    /// <summary>
    /// The simulation settings
    /// </summary>
    public class SimulationSettings
    {
        public int TransactionsSent { get; set; }

        /// <summary>
        /// Ends the simulation after given time
        /// </summary>
        public TimeSpan? ForceEndAfter { get; set; }

        /// <summary>
        /// Sends all transactions in one request
        /// </summary>
        public bool SendTransactionsTogether { get; set; }

        /// <summary>
        /// Dictionary of nodes and number of transactions to be sent
        /// </summary>
        public Dictionary<string, int> NodesAndTransactions { get; set; }
    }
}