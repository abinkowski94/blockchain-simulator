using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IQueuedHostedServiceSynchronizationContext
    {
        Task WaitAsync(int millisecondsTimeout, CancellationToken cancellationToken);

        void Release();
    }
}