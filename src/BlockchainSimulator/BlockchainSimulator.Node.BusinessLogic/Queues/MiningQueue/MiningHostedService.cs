using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace BlockchainSimulator.Node.BusinessLogic.Queues.MiningQueue
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
                    await workItem(cancellationToken);
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