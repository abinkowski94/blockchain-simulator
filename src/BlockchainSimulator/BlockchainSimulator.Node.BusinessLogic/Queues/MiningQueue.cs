using BlockchainSimulator.Node.BusinessLogic.Services;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Extensions;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class MiningQueue : IMiningQueue
    {
        private readonly IStatisticService _statisticService;
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        private readonly ConcurrentQueue<Tuple<DateTime, Func<CancellationToken, Task>>> _workItems =
            new ConcurrentQueue<Tuple<DateTime, Func<CancellationToken, Task>>>();

        public int Length => _workItems.Count;

        public bool IsWorking { get; set; } = true;

        public MiningQueue(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        public async Task<Func<CancellationToken, Task>> DequeueTaskAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);

            _workItems.TryDequeue(out var workItem);
            _statisticService.RegisterQueueTime(DateTime.UtcNow - workItem.Item1);
            _statisticService.RegisterQueueLengthChange(Length);

            return workItem.Item2;
        }

        public void EnqueueTask(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(new Tuple<DateTime, Func<CancellationToken, Task>>(DateTime.UtcNow, workItem));
            _statisticService.RegisterQueueLengthChange(Length);
            _signal.Release();
        }

        public void Clear()
        {
            LinqExtensions.RepeatAction(_workItems.Count, () => _signal.WaitAsync());
            _workItems.Clear();
        }
    }
}