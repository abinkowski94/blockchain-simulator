using BlockchainSimulator.Node.BusinessLogic.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class MiningQueue : IMiningQueue
    {
        private readonly SemaphoreSlim _signal;
        private readonly IStatisticService _statisticService;
        private readonly ConcurrentQueue<Tuple<DateTime, Func<CancellationToken, Task>>> _workItems;

        public int Length => _workItems.Count;

        public MiningQueue(IStatisticService statisticService)
        {
            _workItems = new ConcurrentQueue<Tuple<DateTime, Func<CancellationToken, Task>>>();
            _signal = new SemaphoreSlim(0);
            _statisticService = statisticService;
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            _workItems.TryDequeue(out var workItem);
            _statisticService.RegisterQueueTime(DateTime.UtcNow - workItem.Item1);
            _statisticService.RegisterQueueLengthChange(Length);

            return workItem.Item2;
        }

        public void QueueMiningTask(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(new Tuple<DateTime, Func<CancellationToken, Task>>(DateTime.UtcNow, workItem));
            _statisticService.RegisterQueueLengthChange(Length);
            _signal.Release();
        }
    }
}