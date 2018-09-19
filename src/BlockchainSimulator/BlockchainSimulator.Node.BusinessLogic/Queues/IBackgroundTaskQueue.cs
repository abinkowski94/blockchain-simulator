using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IBackgroundTaskQueue
    {
        int Length { get; }

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);

        void EnqueueTask(Func<CancellationToken, Task> workItem);
    }
}