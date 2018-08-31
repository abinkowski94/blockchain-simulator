using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class MiningQueue : IMiningQueue
    {
        private readonly SemaphoreSlim _signal;
        private readonly ConcurrentQueue<Tuple<DateTime, Func<CancellationToken, Task>>> _workItems;

        public int Length => _workItems.Count;
        public int MaxQueueLength { get; private set; }
        public TimeSpan TotalQueueTime { get; private set; }

        public MiningQueue()
        {
            _workItems = new ConcurrentQueue<Tuple<DateTime, Func<CancellationToken, Task>>>();
            _signal = new SemaphoreSlim(0);

            MaxQueueLength = 0;
            TotalQueueTime = new TimeSpan();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            TotalQueueTime += DateTime.UtcNow - workItem.Item1;

            return workItem.Item2;
        }

        public void QueueMiningTask(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(new Tuple<DateTime, Func<CancellationToken, Task>>(DateTime.UtcNow, workItem));

            if (_workItems.Count > MaxQueueLength)
            {
                MaxQueueLength = _workItems.Count;
            }

            _signal.Release();
        }
    }
}