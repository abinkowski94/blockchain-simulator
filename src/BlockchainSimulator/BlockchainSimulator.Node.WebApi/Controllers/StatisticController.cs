using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.WebApi.Extensions;
using Microsoft.AspNetCore.Mvc;
using Statistic = BlockchainSimulator.Node.BusinessLogic.Model.Statistics.Statistic;

namespace BlockchainSimulator.Node.WebApi.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// The statistic controller
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StatisticController : BaseController
    {
        /// <summary>
        /// The statistics service
        /// </summary>
        private readonly IStatisticService _statisticService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="statisticService">The statistic service</param>
        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        /// <summary>
        /// Gets the statistics
        /// </summary>
        /// <returns>The statistics</returns>
        [HttpGet]
        public ActionResult<BaseResponse> GetStatistics()
        {
            return _statisticService.GetStatistics()
                .GetActionResult<Statistic, Common.Models.Statistics.Statistic>(this);
        }
    }
}