using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Statistics;
using BlockchainSimulator.Node.DataAccess.Model;
using System;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public interface IStatisticService
    {
        void AddBlockchainBranch(Blockchain incomingBlockchain);

        BaseResponse<Statistic> GetStatistics();

        void RegisterAbandonedBlock();

        void RegisterMiningAttempt();

        void RegisterQueueLengthChange(int length);

        void RegisterQueueTime(TimeSpan timespan);

        void RegisterRejectedBlockchain();
    }
}