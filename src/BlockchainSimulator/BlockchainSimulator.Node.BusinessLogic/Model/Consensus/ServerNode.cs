namespace BlockchainSimulator.Node.BusinessLogic.Model.Consensus
{
    public class ServerNode
    {
        public long Delay { get; set; }
        public string HttpAddress { get; set; }
        public string Id { get; set; }
        public bool? IsConnected { get; set; }
    }
}