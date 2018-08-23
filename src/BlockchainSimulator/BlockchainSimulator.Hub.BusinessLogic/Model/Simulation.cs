using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Model
{
    public class Simulation
    {
        public Guid ScenarioId { get; set; }
        
        public IEnumerable<ServerNode> ServerNodes { get; set; }
        
        public DateTime LastRunTime { get; set; }
        
        public SimulationStatuses Status { get; set; }
    }
}