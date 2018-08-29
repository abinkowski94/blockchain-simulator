using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Queues
{
    /// <summary>
    /// The background queue
    /// </summary>
    public interface IBackgroundTaskQueue
    {
        /// <summary>
        /// Dequeue the work item
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The work item</returns>
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Adds background work item to the queue
        /// </summary>
        /// <param name="workItem">The work item</param>
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
    }
}