using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.BusinessLogic.Model.Consensus;
using BlockchainSimulator.BusinessLogic.Model.Responses;
using BlockchainSimulator.BusinessLogic.Queues.BackgroundTasks;
using BlockchainSimulator.BusinessLogic.Services;
using Moq;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Services
{
    public class BaseConsensusServiceTests
    {
        private readonly Mock<IBackgroundTaskQueue> _backgroundTaskQueueMock;
        private readonly IConsensusService _consensusService;

        public BaseConsensusServiceTests()
        {
            _backgroundTaskQueueMock = new Mock<IBackgroundTaskQueue>();
            _consensusService = new Mock<BaseConsensusService>(_backgroundTaskQueueMock.Object)
                {CallBase = true}.Object;
        }

        [Fact]
        public void GetNodes_Empty_SuccessResponseWithNodes()
        {
            // Arrange
            _consensusService.ConnectNode(new ServerNode {Id = "1", HttpAddress = "https://test:4200", Delay = 100});
            _consensusService.ConnectNode(new ServerNode {Id = "2", HttpAddress = "https://test:4200", Delay = 1000});

            // Act
            var result = _consensusService.GetNodes() as SuccessResponse<List<ServerNode>>;

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Result);
            Assert.Contains("The server list for current time:", result.Message);
            Assert.Equal(2, result.Result.Count);
            Assert.Equal("1", result.Result.First().Id);
            Assert.Equal("2", result.Result.Last().Id);
        }

        [Fact]
        public void ConnectNode_Null_ErrorResponse()
        {
            // Arrange

            // Act
            var response = _consensusService.ConnectNode(null) as ErrorResponse<ServerNode>;

            // Assert
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Never);

            Assert.NotNull(response);
            Assert.Null(response.Result);
            Assert.Equal("Invalid input node", response.Message);
            Assert.Single(response.Errors);
            Assert.Equal("The node can not be null!", response.Errors.First());
        }

        [Fact]
        public void ConnectNode_EmptyNode_ErrorResponse()
        {
            // Arrange

            // Act
            var response = _consensusService.ConnectNode(new ServerNode()) as ErrorResponse<ServerNode>;

            // Assert
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Never);

            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.Equal("Invalid input node", response.Message);
            Assert.Equal(2, response.Errors.Length);
            Assert.Contains("The node's id can not be null!", response.Errors);
            Assert.Contains("The node's http address cannot be null!", response.Errors);
        }

        [Fact]
        public void ConnectNode_Node_ErrorResponseAlreadyExists()
        {
            // Arrange
            _consensusService.ConnectNode(new ServerNode {Id = "1", HttpAddress = "http://test:4200"});

            // Act
            var response = _consensusService.ConnectNode(new ServerNode {Id = "1", HttpAddress = "http://test:4200"})
                as ErrorResponse<ServerNode>;

            // Assert
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Once);

            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.Equal("Invalid input node", response.Message);
            Assert.Single(response.Errors);
            Assert.Contains("The node's id already exists id: 1!", response.Errors);
        }

        [Fact]
        public async Task ConnectNode_Node_SuccessResponse()
        {
            // Arrange
            var node = new ServerNode {Id = "1", HttpAddress = "http://test:4200"};

            var token = new CancellationToken();
            Func<CancellationToken, Task> queueTask = null;
            _backgroundTaskQueueMock.Setup(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()))
                .Callback((Func<CancellationToken, Task> func) => queueTask = func);
            
            // Act
            var response = _consensusService.ConnectNode(node) as SuccessResponse<ServerNode>;
            await queueTask(token);

            // Assert
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Once);

            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.Equal("The node has been added successfully!", response.Message);
        }

        [Fact]
        public void DisconnectNode_Id_ErrorResponse()
        {
            // Arrange
            const string id = "1";
            
            // Act
            var result = _consensusService.DisconnectNode(id) as ErrorResponse<ServerNode>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.Null(result.Result);
            Assert.Equal($"Could not disconnect node with id: {id}", result.Message);
        }
        
        [Fact]
        public void DisconnectNode_Id_SuccessResponse()
        {
            // Arrange
            const string id = "1";
            _consensusService.ConnectNode(new ServerNode {Id = id, HttpAddress = "http://test:4200"});
            
            // Act
            var result = _consensusService.DisconnectNode(id) as SuccessResponse<ServerNode>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Result);
            Assert.False(result.Result.IsConnected);
            Assert.Equal($"The node with id: {id} has been disconnected!", result.Message);
        }
        
        [Fact]
        public void DisconnectFromNetwork_Empty_SuccessResponse()
        {
            // Arrange
            _consensusService.ConnectNode(new ServerNode {Id = "1", HttpAddress = "http://test:4200"});
            _consensusService.ConnectNode(new ServerNode {Id = "2", HttpAddress = "http://test:4200"});
            
            // Act
            var result = _consensusService.DisconnectFromNetwork() as SuccessResponse<List<ServerNode>>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Result);
            Assert.Equal(2, result.Result.Count);
            Assert.Equal("All nodes has been disconnected!", result.Message);
        }
    }
}