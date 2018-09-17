using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Queues
{
    public class MiningHostedServiceTests
    {
        private readonly MiningHostedService _hostedService;
        private readonly MiningQueue _queue;

        public MiningHostedServiceTests()
        {
            _queue = new MiningQueue(new Mock<IStatisticService>().Object);
            _hostedService =
                new MiningHostedService(_queue, new Mock<IMiningHostedServiceSynchronizationContext>().Object);
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