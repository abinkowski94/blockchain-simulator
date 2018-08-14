using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Consensus;
using BlockchainSimulator.BusinessLogic.Model.Responses;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IConsensusService
    {
        void ReachConsensus();

        BaseResponse<List<ServerNode>> GetNodes();

        BaseResponse<bool> AcceptBlockchain(string base64Blockchain);

        BaseResponse<ServerNode> ConnectNode(ServerNode serverNode);

        BaseResponse<ServerNode> DisconnectNode(string nodeId);

        BaseResponse<List<ServerNode>> DisconnectFromNetwork();
    }
}