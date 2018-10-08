using BlockchainSimulator.Common.Extensions;
using BlockchainSimulator.Node.BusinessLogic.Services;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class MiningHostedService : BackgroundService
    {
        private readonly IMiningQueue _queue;
        private readonly IMiningService _miningService;

        public MiningHostedService(IMiningQueue queue, IMiningService miningService)
        {
            _queue = queue;
            _miningService = miningService;
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

                    // Set queue working status and re-mine blocks
                    _queue.IsWorking = _queue.Length > 0;
                    _miningService.ReMineAndSynchronizeBlocks();
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