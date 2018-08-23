using System;

namespace BlockchainSimulator.Hub.BusinessLogic.Model
{
    public class Scenario
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public DateTime CreateDate { get; set; }
        
        public DateTime ModificationDate { get; set; }
        
        public Simulation Simulation { get; set; }
    }
}