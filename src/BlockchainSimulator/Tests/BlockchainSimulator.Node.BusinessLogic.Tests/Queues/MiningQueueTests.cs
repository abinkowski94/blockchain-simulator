using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Queues
{
    public class MiningQueueTests
    {
        private readonly BusinessLogic.Queues.MiningQueue _taskQueue;

        public MiningQueueTests()
        {
            _taskQueue = new BusinessLogic.Queues.MiningQueue();
        }

        [Fact]
        public void QueueMiningTask_Task_Void()
        {
            // Arrange

            // Act
            _taskQueue.QueueMiningTask(token => Task.Run(() => Thread.Sleep(100), token));

            // Assert
        }

        [Fact]
        public async Task DequeueAsync_Task_String()
        {
            // Arrange
            var token = new CancellationToken();
            _taskQueue.QueueMiningTask(t => Task.Run(() => Thread.Sleep(100), t));

            // Act
            var result = await _taskQueue.DequeueAsync(token);
            await result(token);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void QueueMiningTask_Null_Exception()
        {
            // Arrange

            // Act
            void Action() => _taskQueue.QueueMiningTask(null);

            // Assert
            Assert.Throws<ArgumentNullException>((Action) Action);
        }
    }
}