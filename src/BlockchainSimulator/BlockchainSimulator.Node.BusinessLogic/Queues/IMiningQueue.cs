using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public interface IMiningQueue
    {
        int Length { get; }

        bool IsWorking { get; set; }

        Task<Func<CancellationToken, Task>> DequeueTaskAsync(CancellationToken cancellationToken);

        void EnqueueTask(Func<CancellationToken, Task> workItem);

        void Clear();
    }
}