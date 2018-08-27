using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Hub.BusinessLogic.Services;
using BlockchainSimulator.Hub.WebApi.Extensions;
using BlockchainSimulator.Hub.WebApi.Model;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace BlockchainSimulator.Hub.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The scenario controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ScenarioController : BaseController
    {
        private readonly IScenarioService _scenarioService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="scenarioService">The scenario service</param>
        public ScenarioController(IScenarioService scenarioService)
        {
            _scenarioService = scenarioService;
        }

        /// <summary>
        /// Creates new scenario
        /// </summary>
        /// <param name="scenario">The scenario to create</param>
        /// <returns>Response with newly created scenario</returns>
        [HttpPost]
        public ActionResult<BaseResponse> CreateScenario([FromBody] Scenario scenario)
        {
            var mappedScenario = LocalMapper.Map<BusinessLogic.Model.Scenario>(scenario);
            var response = _scenarioService.CreateScenario(mappedScenario);

            return response.GetActionResult<BusinessLogic.Model.Scenario, Scenario>(this);
        }

        /// <summary>
        /// Deletes the scenario
        /// </summary>
        /// <param name="id">Id of the scenario</param>
        /// <returns>Deleted scenario</returns>
        [HttpDelete("{id}")]
        public ActionResult<BaseResponse> DeleteScenario(Guid id)
        {
            return _scenarioService.RemoveScenario(id)
                .GetActionResult<BusinessLogic.Model.Scenario, Scenario>(this);
        }

        /// <summary>
        /// Duplicates the scenario
        /// </summary>
        /// <param name="id">Id of the scenario for duplication</param>
        /// <returns>Newly create scenario</returns>
        [HttpPatch("{id}")]
        public ActionResult<BaseResponse> DuplicateScenario(Guid id)
        {
            return _scenarioService.DuplicateScenario(id)
                .GetActionResult<BusinessLogic.Model.Scenario, Scenario>(this);
        }

        /// <summary>
        /// Gets single scenario
        /// </summary>
        /// <param name="id">Id of the scenario</param>
        /// <returns>The response with scenario</returns>
        [HttpGet("{id}")]
        public ActionResult<BaseResponse> GetScenario(Guid id)
        {
            return _scenarioService.GetScenario(id)
                .GetActionResult<BusinessLogic.Model.Scenario, Scenario>(this);
        }

        /// <summary>
        /// Gets the list of scenarios
        /// </summary>
        /// <returns>The response with scenarios</returns>
        [HttpGet]
        public ActionResult<BaseResponse> GetScenarios()
        {
            return _scenarioService.GetScenarios()
                .GetActionResult<List<BusinessLogic.Model.Scenario>, List<Scenario>>(this);
        }

        /// <summary>
        /// Renames the scenario
        /// </summary>
        /// <param name="id">The id of the scenario</param>
        /// <param name="newName">The new name of the scenario</param>
        /// <returns>Updated scenario</returns>
        [HttpPut]
        public ActionResult<BaseResponse> RenameScenario(Guid id, string newName)
        {
            return _scenarioService.RenameScenario(id, newName)
                .GetActionResult<BusinessLogic.Model.Scenario, Scenario>(this);
        }
    }
}