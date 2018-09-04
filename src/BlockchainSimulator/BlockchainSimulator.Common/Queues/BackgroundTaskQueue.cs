using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Queues
{
    /// <inheritdoc />
    /// <summary>
    /// The background queue
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();

        /// <inheritdoc />
        /// <summary>
        /// Dequeue the work item
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The work item</returns>
        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }

        /// <inheritdoc />
        /// <summary>
        /// Adds background work item to the queue
        /// </summary>
        /// <param name="workItem">The work item</param>
        public void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }
    }
}