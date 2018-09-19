using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IQueuedHostedServiceSynchronizationContext _synchronizationContext;
        private readonly IBackgroundTaskQueue _queue;

        public QueuedHostedService(IBackgroundTaskQueue queue,
            IQueuedHostedServiceSynchronizationContext synchronizationContext)
        {
            _queue = queue;
            _synchronizationContext = synchronizationContext;
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
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("The task has been cancelled due to timeout!");
                }
                catch (Exception e)
                {
                    // TODO: log errors
                    Console.WriteLine(e);
                }

                if (_queue.Length <= 0)
                {
                    _synchronizationContext.Release();
                }
            }
        }
    }
}