using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class MiningHostedService : BackgroundService
    {
        private readonly IMiningQueue _taskQueue;
        private readonly IMiningHostedServiceSynchronizationContext _synchronizationContext;
        
        public MiningHostedService(IMiningQueue taskQueue,
            IMiningHostedServiceSynchronizationContext synchronizationContext)
        {
            _taskQueue = taskQueue;
            _synchronizationContext = synchronizationContext;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await _taskQueue.DequeueAsync(cancellationToken);

                try
                {
                    var task = workItem(cancellationToken);
                    if (task.Status == TaskStatus.Created)
                    {
                        task.Start();
                    }

                    await task;
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("The task has been cancelled due to timeout!");
                }
                catch (Exception e)
                {
                    // TODO: log errors
                    Console.WriteLine(e);
                }

                if (_taskQueue.Length == 0)
                {
                    _synchronizationContext.Release();
                }
            }
        }
    }
}