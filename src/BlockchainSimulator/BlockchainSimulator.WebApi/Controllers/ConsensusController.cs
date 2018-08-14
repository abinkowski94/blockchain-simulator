using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.WebApi.Models;
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
        public ActionResult<List<ServerNode>> GetNodes()
        {
            var result = _consensusService.GetNodes();
            return LocalMapper.Map<List<ServerNode>>(result);
        }

        [HttpPost]
        public ActionResult<bool> AcceptBlockchain([FromBody] EncodedBlockchain encodedBlockchain)
        {
            return _consensusService.AcceptBlockchain(encodedBlockchain.Base64Blockchain);
        }

        [HttpPut]
        public ActionResult<ServerNode> ConnectNode([FromBody] ServerNode serverNode)
        {
            var mappedServerNode = LocalMapper.Map<BusinessLogic.Model.Consensus.ServerNode>(serverNode);
            var result = _consensusService.ConnectNode(mappedServerNode);

            return LocalMapper.Map<ServerNode>(result);
        }

        [HttpDelete]
        public ActionResult<ServerNode> DisconnectNode(string nodeId)
        {
            var result = _consensusService.DisconnectNode(nodeId);
            return LocalMapper.Map<ServerNode>(result);
        }

        [HttpPatch]
        public ActionResult<List<ServerNode>> DisconnectFromNetwork()
        {
            var result = _consensusService.DisconnectFromNetwork();
            return LocalMapper.Map<List<ServerNode>>(result);
        }
    }
}