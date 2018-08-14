using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Consensus;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IConsensusService
    {
        void ReachConsensus();

        List<ServerNode> GetNodes();

        bool AcceptBlockchain(string base64Blockchain);

        ServerNode ConnectNode(ServerNode serverNode);

        ServerNode DisconnectNode(string nodeId);

        List<ServerNode> DisconnectFromNetwork();
    }
}