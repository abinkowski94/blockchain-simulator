using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Queues
{
    /// <inheritdoc />
    /// <summary>
    /// The queued hosted service synchronization context
    /// </summary>
    public class QueuedHostedServiceSynchronizationContext : IQueuedHostedServiceSynchronizationContext
    {
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);
        
        /// <inheritdoc />
        /// <summary>
        /// Waits for the semaphore
        /// </summary>
        /// <returns>Waiting task</returns>
        public Task WaitAsync()
        {
            return _signal.WaitAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Releases semaphore
        /// </summary>
        public void Release()
        {
            _signal.Release();
        }
    }
}