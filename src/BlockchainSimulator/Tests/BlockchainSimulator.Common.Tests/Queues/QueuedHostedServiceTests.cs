using BlockchainSimulator.Common.Queues;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BlockchainSimulator.Common.Tests.Queues
{
    public class QueuedHostedServiceTests
    {
        private readonly QueuedHostedService _hostedService;
        private readonly BackgroundTaskQueue _queue;

        public QueuedHostedServiceTests()
        {
            _queue = new BackgroundTaskQueue();
            _hostedService = new QueuedHostedService(_queue);
        }

        [Fact]
        public async Task StartStop_Token_Void()
        {
            // Arrange
            var token = new CancellationToken();
            _queue.QueueBackgroundWorkItem(t => Task.Run(() => Thread.Sleep(100), t));

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
            _queue.QueueBackgroundWorkItem(t => Task.Run(() => throw new Exception(), t));

            // Act
            await _hostedService.StartAsync(token);
            await _hostedService.StopAsync(token);

            // Assert
        }
    }
}