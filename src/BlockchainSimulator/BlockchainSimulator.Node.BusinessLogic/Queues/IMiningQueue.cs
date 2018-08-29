using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IMiningQueue
    {
        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void QueueMiningTask(Func<CancellationToken, Task> workItem);
    }
}