using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Queues
{
    public class MiningQueueTests
    {
        private readonly MiningQueue _taskQueue;

        public MiningQueueTests()
        {
            _taskQueue = new MiningQueue(new Mock<IStatisticService>().Object);
        }

        [Fact]
        public async Task DequeueAsync_Task_String()
        {
            // Arrange
            var token = new CancellationToken();
            _taskQueue.EnqueueTask(t => Task.Run(() => Thread.Sleep(100), t));

            // Act
            var result = await _taskQueue.DequeueTaskAsync(token);
            await result(token);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void QueueMiningTask_Null_Exception()
        {
            // Arrange

            // Act
            void Action() => _taskQueue.EnqueueTask(null);

            // Assert
            Assert.Throws<ArgumentNullException>((Action)Action);
        }

        [Fact]
        public void QueueMiningTask_Task_Void()
        {
            // Arrange

            // Act
            _taskQueue.EnqueueTask(token => Task.Run(() => Thread.Sleep(100), token));

            // Assert
        }
    }
}