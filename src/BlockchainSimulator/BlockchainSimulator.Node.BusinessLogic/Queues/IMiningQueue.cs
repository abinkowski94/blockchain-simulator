using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IMiningQueue
    {
        int Length { get; }
        int MaxQueueLength { get; }
        TimeSpan TotalQueueTime { get; }

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void QueueMiningTask(Func<CancellationToken, Task> workItem);
    }
}