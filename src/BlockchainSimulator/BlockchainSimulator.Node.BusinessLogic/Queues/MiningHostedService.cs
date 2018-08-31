using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Queues
{
    public class MiningHostedService : BackgroundService
    {
        private readonly IMiningQueue _taskQueue;

        public MiningHostedService(IMiningQueue taskQueue)
        {
            _taskQueue = taskQueue;
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
                catch (Exception e)
                {
                    // TODO: log errors
                    Console.WriteLine(e);
                }
            }
        }
    }
}