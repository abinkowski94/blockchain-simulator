using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Queues
{
    /// <summary>
    /// The queued hosted service synchronization context
    /// </summary>
    public interface IQueuedHostedServiceSynchronizationContext
    {
        /// <summary>
        /// Waits for the semaphore
        /// </summary>
        /// <returns>Waiting task</returns>
        Task WaitAsync();

        /// <summary>
        /// Releases semaphore
        /// </summary>
        void Release();
    }
}