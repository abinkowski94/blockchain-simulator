using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Queues
{
    /// <inheritdoc />
    /// <summary>
    /// The queue hosted service
    /// </summary>
    public class QueuedHostedService : BackgroundService
    {
        private readonly IQueuedHostedServiceSynchronizationContext _synchronizationContext;
        private readonly IBackgroundTaskQueue _taskQueue;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="taskQueue">The queue</param>
        /// <param name="synchronizationContext">The synchronization context</param>
        public QueuedHostedService(IBackgroundTaskQueue taskQueue,
            IQueuedHostedServiceSynchronizationContext synchronizationContext)
        {
            _taskQueue = taskQueue;
            _synchronizationContext = synchronizationContext;
        }

        /// <inheritdoc />
        /// <summary>
        /// Execute method
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Task</returns>
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
                catch (TaskCanceledException)
                {
                    Console.WriteLine("The task has been cancelled due to timeout!");
                }
                catch (Exception e)
                {
                    // TODO: log errors
                    Console.WriteLine(e);
                }

                if (_taskQueue.Length <= 0)
                {
                    _synchronizationContext.Release();
                }
            }
        }
    }
}