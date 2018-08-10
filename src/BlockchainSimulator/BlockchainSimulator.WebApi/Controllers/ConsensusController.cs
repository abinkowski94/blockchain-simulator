using System;
using System.Collections.Generic;
using BlockchainSimulator.WebApi.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConsensusController : BaseController
    {
        [HttpGet]
        public ActionResult<List<ServerNode>> GetNodes()
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public ActionResult AcceptBlock(string base64Blockchain)
        {
            throw new NotImplementedException();
        }

        [HttpPut]
        public ActionResult<ServerNode> ConnectNode([FromBody] ServerNode serverNode)
        {
            throw new NotImplementedException();
        }

        [HttpDelete]
        public ActionResult<ServerNode> DisconnectNode(string nodeId)
        {
            throw new NotImplementedException();
        }
    }
}