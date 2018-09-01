using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Node.BusinessLogic.Queues;
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
        private readonly IMiningQueue _miningQueue;
        private readonly IStatisticService _statisticService;

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="miningQueue">The mining queue</param>
        /// <param name="statisticService">The statistic service</param>
        public StatisticController(IMiningQueue miningQueue, IStatisticService statisticService)
        {
            _miningQueue = miningQueue;
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

        /// <summary>
        /// Gets the status of mining queue
        /// </summary>
        /// <returns>Status of the mining queue</returns>
        [HttpGet("mining-queue")]
        public ActionResult<MiningQueueStatus> GetStatus()
        {
            return new MiningQueueStatus {Length = _miningQueue.Length};
        }
    }
}