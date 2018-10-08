using Microsoft.AspNetCore.SignalR.Client;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Consensus
{
    public class ServerNode
    {
        public HubConnection HubConnection { get; set; }
        public long Delay { get; set; }
        public string HttpAddress { get; set; }
        public string Id { get; set; }
        public bool? IsConnected { get; set; }
    }
}