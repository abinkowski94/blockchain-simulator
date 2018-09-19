using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class QueuedHostedServiceSynchronizationContext : IQueuedHostedServiceSynchronizationContext
    {
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public Task WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken)
        {
            return _signal.WaitAsync(millisecondsTimeout, cancellationToken);
        }

        public void Release()
        {
            _signal.Release();
        }
    }
}