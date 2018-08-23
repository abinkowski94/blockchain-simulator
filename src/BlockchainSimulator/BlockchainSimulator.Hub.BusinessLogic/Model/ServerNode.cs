using System.Collections.Generic;

namespace BlockchainSimulator.Hub.BusinessLogic.Model
{
    public class ServerNode
    {
        public long Delay { get; set; }

        public string HttpAddress { get; set; }

        public string Id { get; set; }

        public bool? IsConnected { get; set; }
        
        public IEnumerable<string> ConnectedTo { get; set; }
    }
}