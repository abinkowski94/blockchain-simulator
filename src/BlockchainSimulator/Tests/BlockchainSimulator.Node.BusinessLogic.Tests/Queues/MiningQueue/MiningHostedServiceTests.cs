using System;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Queues.MiningQueue;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Queues.MiningQueue
{
    public class MiningHostedServiceTests
    {
        private readonly MiningHostedService _hostedService;
        private readonly BusinessLogic.Queues.MiningQueue.MiningQueue _queue;

        public MiningHostedServiceTests()
        {
            _queue = new BusinessLogic.Queues.MiningQueue.MiningQueue();
            _hostedService = new MiningHostedService(_queue);
        }

        [Fact]
        public async Task StartStop_Token_Void()
        {
            // Arrange
            var token = new CancellationToken();
            _queue.QueueMiningTask(t => Task.Run(() => Thread.Sleep(100), t));

            // Act
            await _hostedService.StartAsync(token);
            await _hostedService.StopAsync(token);

            // Assert
        }

        [Fact]
        public async Task StartStopWithException_Token_Void()
        {
            // Arrange
            var token = new CancellationToken();
            _queue.QueueMiningTask(t => Task.Run(() => throw new Exception(), t));

            // Act
            await _hostedService.StartAsync(token);
            await _hostedService.StopAsync(token);

            // Assert
        }
    }
}