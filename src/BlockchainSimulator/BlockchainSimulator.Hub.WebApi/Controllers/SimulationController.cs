using System;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Hub.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

namespace BlockchainSimulator.Hub.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The simulation controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SimulationController : BaseController
    {
        /// <summary>
        /// Adds new node to the simulation
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="serverNode">The node to be added</param>
        /// <returns>The newly added node</returns>
        [HttpPost]
        public ActionResult<BaseResponse> AddNode(Guid scenarioId, [FromBody] ServerNode serverNode)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Removes the with given id
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="id">Id of the node to be removed</param>
        /// <returns>Removed node</returns>
        [HttpDelete("{id}")]
        public ActionResult<BaseResponse> DeleteNode(string id, Guid scenarioId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Connects two nodes
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="id1">Id of first node</param>
        /// <param name="id2">Id of second node</param>
        /// <returns>Response with connected nodes</returns>
        [HttpPut]
        public ActionResult<BaseResponse> ConnectNodes(Guid scenarioId, string id1, string id2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Starts the simulation
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="settings">The start settings of the simulation</param>
        /// <returns>The started simulation</returns>
        [HttpPost("start/{scenarioId}")]
        public ActionResult<BaseResponse> Start(Guid scenarioId, [FromBody] SimulationSettings settings)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Generates and gets the statistic of the simulation basing on last run
        /// </summary>
        /// <param name="scenarioId">The scenario id</param>
        /// <returns>Statistics for the simulation</returns>
        [HttpGet("statistics/{scenarioId}")]
        public ActionResult<BaseResponse> GenerateAndGetStatistic(Guid scenarioId)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets all statistics of the simulation
        /// </summary>
        /// <param name="scenarioId">The scenario id</param>
        /// <returns>Statistics for the simulation</returns>
        [HttpGet("statistics/{scenarioId}")]
        public ActionResult<BaseResponse> GetAllStatistics(Guid scenarioId)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes all statistics of the simulation
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <returns>Deleted statistics</returns>
        [HttpDelete("statistics/{scenarioId}")]
        public ActionResult<BaseResponse> DeleteStatistics(Guid scenarioId)
        {
            throw new NotImplementedException();
        }
    }
}