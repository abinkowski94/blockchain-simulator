using BlockchainSimulator.Common.Extensions;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class MiningHostedService : BackgroundService
    {
        private readonly IMiningQueue _queue;

        public MiningHostedService(IMiningQueue queue)
        {
            _queue = queue;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                // Dequeue task
                var workItem = await _queue.DequeueTaskAsync(cancellationToken);
                _queue.IsWorking = true;

                try
                {
                    // Start task if is in state created
                    var task = workItem(cancellationToken);
                    if (task.Status == TaskStatus.Created)
                    {
                        task.Start();
                    }

                    // Execute task
                    await task;

                    // Set queue working status
                    _queue.IsWorking = _queue.Length > 0;
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine("The task has been cancelled due to timeout!");
                }
                catch (AggregateException exception)
                {
                    Console.WriteLine("Many errors occurred during the operation!");
                    exception.InnerExceptions.ForEach(e => Console.WriteLine(e.Message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}