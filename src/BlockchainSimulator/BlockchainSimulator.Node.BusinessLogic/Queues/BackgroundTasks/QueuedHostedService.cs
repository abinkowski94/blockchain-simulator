using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BlockchainSimulator.Node.BusinessLogic.Queues.BackgroundTasks
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IBackgroundTaskQueue _taskQueue;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue)
        {
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(cancellationToken);

                try
                {
                    var task = workItem(cancellationToken);
                    if (task.Status == TaskStatus.Created)
                    {
                        task.Start();
                    }

                    await task;
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