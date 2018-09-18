using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.WebApi.Extensions;
using BlockchainSimulator.Node.WebApi.Models.Blockchain;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.WebApi.Controllers
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
        /// Checks and accepts the incoming block and adds if it is valid
        /// </summary>
        /// <param name="encodedBlock">The encoded block</param>
        /// <returns>The response if the block has been accepted or not</returns>
        [HttpPost]
        public void AcceptBlockchain([FromBody] EncodedBlock encodedBlock)
        {
            _consensusService.AcceptBlock(encodedBlock);
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

            return result.GetActionResult<BusinessLogic.Model.Consensus.ServerNode, ServerNode>(this);
        }

        /// <summary>
        /// Disconnects from the network (disconnects all nodes)
        /// </summary>
        /// <returns>List of the disconnected nodes</returns>
        [HttpPatch]
        public ActionResult<BaseResponse> DisconnectFromNetwork()
        {
            var result = _consensusService.DisconnectFromNetwork();
            return result.GetActionResult<List<BusinessLogic.Model.Consensus.ServerNode>, List<ServerNode>>(this);
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
            return result.GetActionResult<BusinessLogic.Model.Consensus.ServerNode, ServerNode>(this);
        }

        /// <summary>
        /// Gets the list of connected nodes
        /// </summary>
        /// <returns>List of the connected nodes</returns>
        [HttpGet]
        public ActionResult<BaseResponse> GetNodes()
        {
            var result = _consensusService.GetNodes();
            return result.GetActionResult<List<BusinessLogic.Model.Consensus.ServerNode>, List<ServerNode>>(this);
        }
    }
}