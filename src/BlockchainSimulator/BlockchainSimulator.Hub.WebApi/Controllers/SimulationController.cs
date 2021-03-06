using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Services;
using BlockchainSimulator.Hub.WebApi.Extensions;
using BlockchainSimulator.Hub.WebApi.Model.Scenarios;
using Microsoft.AspNetCore.Mvc;
using System;

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
        private readonly ISimulationService _simulationService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="simulationService">The simulation service</param>
        public SimulationController(ISimulationService simulationService)
        {
            _simulationService = simulationService;
        }

        /// <summary>
        /// Adds new node to the simulation
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="serverNode">The node to be added</param>
        /// <returns>The newly added node</returns>
        [HttpPost]
        public ActionResult<BaseResponse> AddNode(Guid scenarioId, [FromBody] ServerNode serverNode)
        {
            var mappedNode = LocalMapper.Map<BusinessLogic.Model.Consensus.ServerNode>(serverNode);
            return _simulationService.AddNode(scenarioId, mappedNode)
                .GetActionResult<BusinessLogic.Model.Scenarios.Simulation, Simulation>(this);
        }

        /// <summary>
        /// Changes simulation configuration
        /// </summary>
        /// <param name="scenarioId">The id of scenario</param>
        /// <param name="configuration">The configuration</param>
        /// <returns>Changed simulation</returns>
        [HttpPatch]
        public ActionResult<BaseResponse> ChangeConfiguration(Guid scenarioId,
            [FromBody] BlockchainConfiguration configuration)
        {
            var mappedConfiguration = LocalMapper.Map<BusinessLogic.Model.Scenarios.BlockchainConfiguration>(configuration);
            return _simulationService.ChangeConfiguration(scenarioId, mappedConfiguration)
                .GetActionResult<BusinessLogic.Model.Scenarios.Simulation, Simulation>(this);
        }

        /// <summary>
        /// Connects two nodes
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="nodeId1">Id of first node</param>
        /// <param name="nodeId2">Id of second node</param>
        /// <returns>Response with connected nodes</returns>
        [HttpPut]
        public ActionResult<BaseResponse> ConnectNodes(Guid scenarioId, string nodeId1, string nodeId2)
        {
            return _simulationService.ConnectNodes(scenarioId, nodeId1, nodeId2)
                .GetActionResult<BusinessLogic.Model.Scenarios.Simulation, Simulation>(this);
        }

        /// <summary>
        /// Removes the with given id
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="nodeId">Id of the node to be removed</param>
        /// <returns>Removed node</returns>
        [HttpDelete]
        public ActionResult<BaseResponse> DeleteNode(Guid scenarioId, string nodeId)
        {
            return _simulationService.DeleteNode(scenarioId, nodeId)
                .GetActionResult<BusinessLogic.Model.Scenarios.Simulation, Simulation>(this);
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
        /// Starts the simulation
        /// </summary>
        /// <param name="scenarioId">The id of the scenario</param>
        /// <param name="settings">The start settings of the simulation</param>
        /// <returns>The started simulation</returns>
        [HttpPost("start/{scenarioId}")]
        public ActionResult<BaseResponse> Start(Guid scenarioId, [FromBody] SimulationSettings settings)
        {
            var mappedSetting = LocalMapper.Map<BusinessLogic.Model.Scenarios.SimulationSettings>(settings);
            return _simulationService.StartSimulation(scenarioId, mappedSetting)
                .GetActionResult<BusinessLogic.Model.Scenarios.Simulation, Simulation>(this);
        }
    }
}