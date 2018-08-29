using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Consensus;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IConsensusService
    {
        BaseResponse<bool> AcceptBlockchain(string base64Blockchain);

        BaseResponse<bool> AcceptBlockchain(BlockBase blockBase);

        BaseResponse<ServerNode> ConnectNode(ServerNode serverNode);

        BaseResponse<List<ServerNode>> DisconnectFromNetwork();

        BaseResponse<ServerNode> DisconnectNode(string nodeId);

        BaseResponse<List<ServerNode>> GetNodes();

        void ReachConsensus();
    }
}