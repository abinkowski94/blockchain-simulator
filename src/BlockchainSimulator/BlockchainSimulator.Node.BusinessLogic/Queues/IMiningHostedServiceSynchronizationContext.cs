using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IMiningHostedServiceSynchronizationContext
    {
        Task WaitAsync();

        void Release();
    }
}