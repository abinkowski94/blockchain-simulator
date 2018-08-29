using BlockchainSimulator.Common.Queues;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BlockchainSimulator.Common.Tests.Queues
{
    public class BackgroundTaskQueueTests
    {
        private readonly BackgroundTaskQueue _taskQueue;

        public BackgroundTaskQueueTests()
        {
            _taskQueue = new BackgroundTaskQueue();
        }

        [Fact]
        public async Task DequeueAsync_Task_String()
        {
            // Arrange
            var token = new CancellationToken();
            _taskQueue.QueueBackgroundWorkItem(t => Task.Run(() => Thread.Sleep(100), t));

            // Act
            var result = await _taskQueue.DequeueAsync(token);
            await result(token);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void QueueBackgroundWorkItem_Null_Void()
        {
            // Arrange

            // Act
            void Action() => _taskQueue.QueueBackgroundWorkItem(null);

            // Assert
            Assert.Throws<ArgumentNullException>((Action)Action);
        }

        [Fact]
        public void QueueBackgroundWorkItem_Task_Void()
        {
            // Arrange

            // Act
            _taskQueue.QueueBackgroundWorkItem(token => Task.Run(() => Thread.Sleep(100), token));

            // Assert
        }
    }
}