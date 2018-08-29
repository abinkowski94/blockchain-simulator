using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Consensus;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IConsensusService
    {
        void ReachConsensus();

        BaseResponse<List<ServerNode>> GetNodes();

        BaseResponse<bool> AcceptBlockchain(string base64Blockchain);

        BaseResponse<bool> AcceptBlockchain(BlockBase blockBase);

        BaseResponse<ServerNode> ConnectNode(ServerNode serverNode);

        BaseResponse<ServerNode> DisconnectNode(string nodeId);

        BaseResponse<List<ServerNode>> DisconnectFromNetwork();
    }
}