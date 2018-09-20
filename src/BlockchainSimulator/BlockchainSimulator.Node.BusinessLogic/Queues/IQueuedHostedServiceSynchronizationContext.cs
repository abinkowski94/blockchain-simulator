using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IQueuedHostedServiceSynchronizationContext
    {
        Task WaitAsync(CancellationToken cancellationToken, int millisecondsTimeout = -1);

        void Release();
    }
}