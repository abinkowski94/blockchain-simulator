using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IQueuedHostedServiceSynchronizationContext
    {
        Task WaitAsync();

        void Release();
    }
}