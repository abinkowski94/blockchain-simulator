using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using System.Collections.Generic;
using ServerNode = BlockchainSimulator.Node.BusinessLogic.Model.Consensus.ServerNode;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IConsensusService
    {
        void AcceptExternalBlock(EncodedBlock encodedBlock);

        BaseResponse<bool> AcceptBlock(BlockBase blockBase);

        BaseResponse<ServerNode> ConnectNode(ServerNode serverNode);

        BaseResponse<List<ServerNode>> DisconnectFromNetwork();

        BaseResponse<ServerNode> DisconnectNode(string nodeId);

        BaseResponse<List<ServerNode>> GetNodes();

        BaseResponse<bool> SynchronizeWithOtherNodes();
    }
}