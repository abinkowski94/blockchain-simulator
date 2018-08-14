using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.WebApi.Extensions;
using BlockchainSimulator.WebApi.Models;
using BlockchainSimulator.WebApi.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsensusController : BaseController
    {
        private readonly IConsensusService _consensusService;

        public ConsensusController(IConsensusService consensusService)
        {
            _consensusService = consensusService;
        }

        [HttpGet]
        public ActionResult<BaseResponse> GetNodes()
        {
            var result = _consensusService.GetNodes();
            return result.GetBaseResponse<List<BusinessLogic.Model.Consensus.ServerNode>, List<ServerNode>>(this);
        }

        [HttpPost]
        public ActionResult<BaseResponse> AcceptBlockchain([FromBody] EncodedBlockchain encodedBlockchain)
        {
            return _consensusService.AcceptBlockchain(encodedBlockchain.Base64Blockchain)
                .GetBaseResponse<bool, bool>(this);
        }

        [HttpPut]
        public ActionResult<BaseResponse> ConnectNode([FromBody] ServerNode serverNode)
        {
            var mappedServerNode = LocalMapper.Map<BusinessLogic.Model.Consensus.ServerNode>(serverNode);
            var result = _consensusService.ConnectNode(mappedServerNode);

            return result.GetBaseResponse<BusinessLogic.Model.Consensus.ServerNode, ServerNode>(this);
        }

        [HttpDelete]
        public ActionResult<BaseResponse> DisconnectNode(string nodeId)
        {
            var result = _consensusService.DisconnectNode(nodeId);
            return result.GetBaseResponse<BusinessLogic.Model.Consensus.ServerNode, ServerNode>(this);
        }

        [HttpPatch]
        public ActionResult<BaseResponse> DisconnectFromNetwork()
        {
            var result = _consensusService.DisconnectFromNetwork();
            return result.GetBaseResponse<List<BusinessLogic.Model.Consensus.ServerNode>, List<ServerNode>>(this);
        }
    }
}