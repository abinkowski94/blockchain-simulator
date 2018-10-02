using BlockchainSimulator.Common.Models.Statistics;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using System;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IStatisticService
    {
        BaseResponse<Model.Statistics.Statistic> GetStatistics();

        void RegisterMiningAttempt();

        void RegisterQueueLengthChange(int length);

        void RegisterQueueTime(TimeSpan timespan);

        void RegisterRejectedBlock();

        void RegisterAbandonedBlock();

        void RegisterWork(bool isWorking);

        MiningQueueStatus GetStatus();
    }
}