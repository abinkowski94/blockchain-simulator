using System;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Hub.WebApi.Model;
using Microsoft.AspNetCore.Mvc;

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
        /// <summary>
        /// Gets the single scenario
        /// </summary>
        /// <param name="id">Id of the scenario</param>
        /// <returns>The response with scenario</returns>
        [HttpGet("{id}")]
        public ActionResult<BaseResponse> GetScenario(Guid id)
        {
            throw new NotImplementedException();
        }
        
        /// <summary>
        /// Gets the list of scenarios
        /// </summary>
        /// <returns>The response with scenarios</returns>
        [HttpGet]
        public ActionResult<BaseResponse> GetScenarios()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates new scenario
        /// </summary>
        /// <param name="scenario">The scenario to create</param>
        /// <returns>Response with newly created scenario</returns>
        [HttpPost]
        public ActionResult<BaseResponse> CreateScenario([FromBody] Scenario scenario)
        {
            throw  new NotImplementedException();
        }
        
        /// <summary>
        /// Updates the scenario
        /// </summary>
        /// <param name="scenario">The scenario to update</param>
        /// <returns>Updated scenario</returns>
        [HttpPut]
        public ActionResult<BaseResponse> UpdateScenario([FromBody] Scenario scenario)
        {
            throw  new NotImplementedException();
        }
        
        /// <summary>
        /// Deletes the scenario
        /// </summary>
        /// <param name="id">Id of the scenario</param>
        /// <returns>Deleted scenario</returns>
        [HttpDelete("{id}")]
        public ActionResult<BaseResponse> DeleteScenario(Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Duplicates the scenario
        /// </summary>
        /// <param name="id">Id of the scenario for duplication</param>
        /// <returns>Newly create scenario</returns>
        [HttpPatch("{id}")]
        public ActionResult<BaseResponse> DuplicateScenario(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}