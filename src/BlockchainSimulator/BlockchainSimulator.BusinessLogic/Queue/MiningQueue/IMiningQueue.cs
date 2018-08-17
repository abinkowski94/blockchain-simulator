using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.BusinessLogic.Queue.MiningQueue
{
    public interface IMiningQueue
    {
        void QueueMiningTask(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}