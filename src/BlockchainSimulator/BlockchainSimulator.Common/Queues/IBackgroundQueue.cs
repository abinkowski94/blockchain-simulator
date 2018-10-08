using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Queues
{
    /// <summary>
    /// The background queue
    /// </summary>
    public interface IBackgroundQueue
    {
        /// <summary>
        /// The length of the queue
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Indicates whether the queue is working
        /// </summary>
        bool IsWorking { get; set; }

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
        void Enqueue(Func<CancellationToken, Task> workItem);

        /// <summary>
        /// Clears the queue
        /// </summary>
        void Clear();
    }
}