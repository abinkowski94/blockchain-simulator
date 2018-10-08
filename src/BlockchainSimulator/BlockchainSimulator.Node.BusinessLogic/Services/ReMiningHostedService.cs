using System;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using Microsoft.Extensions.Hosting;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class ReMiningHostedService : BackgroundService
    {
        private readonly IMiningService _miningService;
        private readonly IMiningQueue _miningQueue;
        private readonly IBackgroundQueue _backgroundQueue;
        private readonly IStatisticService _statisticService;

        public ReMiningHostedService(IMiningService miningService, IMiningQueue miningQueue,
            IBackgroundQueue backgroundQueue, IStatisticService statisticService)
        {
            _miningService = miningService;
            _miningQueue = miningQueue;
            _backgroundQueue = backgroundQueue;
            _statisticService = statisticService;
        }

        protected override Task ExecuteAsync(CancellationToken cancellationToken)
        {
            bool IsWorking() => _miningQueue.IsWorking || _backgroundQueue.IsWorking;
            var lastMiningQueueWorkingValue = false;
            var lastBackgroundWorkingValue = false;

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var mining = lastMiningQueueWorkingValue;
                    var background = lastBackgroundWorkingValue;

                    SpinWait.SpinUntil(() =>
                        mining != _miningQueue.IsWorking || background != _backgroundQueue.IsWorking);

                    lastMiningQueueWorkingValue = _miningQueue.IsWorking;
                    lastBackgroundWorkingValue = _backgroundQueue.IsWorking;

                    if (!IsWorking())
                    {
                        _statisticService.RegisterWorkingStatus(true);
                        _miningService.ReMineAndSynchronizeBlocks();
                    }
                    _statisticService.RegisterWorkingStatus(IsWorking());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return Task.CompletedTask;
        }
    }
}