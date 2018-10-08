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
        private readonly IBackgroundQueue _taskQueue;

        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="taskQueue">The queue</param>
        public QueuedHostedService(IBackgroundQueue taskQueue)
        {
            _taskQueue = taskQueue;
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
                // Dequeue task and set working true
                var workItem = await _taskQueue.DequeueAsync(cancellationToken);
                _taskQueue.IsWorking = true;

                try
                {
                    var task = workItem(cancellationToken);
                    if (task.Status == TaskStatus.Created)
                    {
                        task.Start();
                    }

                    // Execute task
                    await task;

                    _taskQueue.IsWorking = _taskQueue.Length > 0;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("The task has been cancelled due to timeout!");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }
    }
}