using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Hub.BusinessLogic.Queues
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);

        Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
    }
}