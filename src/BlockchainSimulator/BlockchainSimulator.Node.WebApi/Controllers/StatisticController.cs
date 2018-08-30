using System;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using Microsoft.AspNetCore.Mvc;

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

        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="miningQueue">The mining queue</param>
        public StatisticController(IMiningQueue miningQueue)
        {
            _miningQueue = miningQueue;
        }

        /// <summary>
        /// Gets the statistics
        /// </summary>
        /// <returns>The statistics</returns>
        [HttpGet]
        public ActionResult<BaseResponse> GetStatistics()
        {
            throw new NotImplementedException();
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