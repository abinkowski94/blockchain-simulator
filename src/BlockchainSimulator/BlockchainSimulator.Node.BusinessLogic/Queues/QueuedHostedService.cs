using BlockchainSimulator.Node.BusinessLogic.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IMiningService _miningService;

        public QueuedHostedService(IBackgroundTaskQueue queue, IMiningService miningService)
        {
            _queue = queue;
            _miningService = miningService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(cancellationToken);

                try
                {
                    var task = workItem(cancellationToken);
                    if (task.Status == TaskStatus.Created)
                    {
                        task.Start();
                    }

                    await task;

                    _miningService.ReMineBlocks();
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("The task has been cancelled due to timeout!");
                }
                catch (Exception e)
                {
                    // TODO: log errors
                    Console.WriteLine(e);
                }
            }
        }
    }
}