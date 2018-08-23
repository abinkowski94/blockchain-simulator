namespace BlockchainSimulator.Node.BusinessLogic.Model.Consensus
{
    public class ServerNode
    {
        public string Id { get; set; }
        
        public string HttpAddress { get; set; }

        public bool? IsConnected { get; set; }

        public long Delay { get; set; }
    }
}