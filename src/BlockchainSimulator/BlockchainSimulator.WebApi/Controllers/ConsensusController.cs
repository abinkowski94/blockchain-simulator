using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Services;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.WebApi.Extensions;
using BlockchainSimulator.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The consensus controller
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConsensusController : BaseController
    {
        private readonly IConsensusService _consensusService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="consensusService">The consensus service</param>
        public ConsensusController(IConsensusService consensusService)
        {
            _consensusService = consensusService;
        }

        /// <summary>
        /// Gets the list of connected nodes
        /// </summary>
        /// <returns>List of the connected nodes</returns>
        [HttpGet]
        public ActionResult<BaseResponse> GetNodes()
        {
            var result = _consensusService.GetNodes();
            return result.GetBaseResponse<List<BusinessLogic.Model.Consensus.ServerNode>, List<ServerNode>>(this);
        }

        /// <summary>
        /// Checks and accepts the incoming blockchain and replaces if is newer
        /// </summary>
        /// <param name="encodedBlockchain">The encoded blockchain</param>
        /// <returns>The response if the blockchain has been accepted or not</returns>
        [HttpPost]
        public ActionResult<BaseResponse> AcceptBlockchain([FromBody] EncodedBlockchain encodedBlockchain)
        {
            return _consensusService.AcceptBlockchain(encodedBlockchain.Base64Blockchain)
                .GetBaseResponse<bool, bool>(this);
        }

        /// <summary>
        /// Allows to connect new node one way communication
        /// </summary>
        /// <param name="serverNode">The node</param>
        /// <returns>The connected node</returns>
        [HttpPut]
        public ActionResult<BaseResponse> ConnectNode([FromBody] ServerNode serverNode)
        {
            var mappedServerNode = LocalMapper.Map<BusinessLogic.Model.Consensus.ServerNode>(serverNode);
            var result = _consensusService.ConnectNode(mappedServerNode);

            return result.GetBaseResponse<BusinessLogic.Model.Consensus.ServerNode, ServerNode>(this);
        }

        /// <summary>
        /// Disconnects the specific node
        /// </summary>
        /// <param name="nodeId">The node id</param>
        /// <returns>Disconnected node</returns>
        [HttpDelete]
        public ActionResult<BaseResponse> DisconnectNode(string nodeId)
        {
            var result = _consensusService.DisconnectNode(nodeId);
            return result.GetBaseResponse<BusinessLogic.Model.Consensus.ServerNode, ServerNode>(this);
        }

        /// <summary>
        /// Disconnects from the network (disconnects all nodes)
        /// </summary>
        /// <returns>List of the disconnected nodes</returns>
        [HttpPatch]
        public ActionResult<BaseResponse> DisconnectFromNetwork()
        {
            var result = _consensusService.DisconnectFromNetwork();
            return result.GetBaseResponse<List<BusinessLogic.Model.Consensus.ServerNode>, List<ServerNode>>(this);
        }
    }
}