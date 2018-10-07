using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Consensus;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Queues;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Services
{
    public class BaseConsensusServiceTests
    {
        private readonly Mock<IBackgroundTaskQueue> _backgroundTaskQueueMock;
        private readonly Mock<IHttpService> _httpServiceMock;
        private readonly IConsensusService _consensusService;

        public BaseConsensusServiceTests()
        {
            _backgroundTaskQueueMock = new Mock<IBackgroundTaskQueue>();
            _httpServiceMock = new Mock<IHttpService>();
            _consensusService = new Mock<BaseConsensusService>(_backgroundTaskQueueMock.Object, _httpServiceMock.Object)
            { CallBase = true }.Object;
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
            _consensusService.ConnectNode(new ServerNode { Id = "1", HttpAddress = "http://test:4200" });

            // Act
            var response = _consensusService.ConnectNode(new ServerNode { Id = "1", HttpAddress = "http://test:4200" })
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
            var serverNode = new ServerNode { Id = "1", HttpAddress = "http://test:4200" };

            var token = new CancellationToken();
            Func<CancellationToken, Task> queueTask = null;
            _backgroundTaskQueueMock.Setup(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()))
                .Callback((Func<CancellationToken, Task> func) => queueTask = func);

            _httpServiceMock.Setup(p => p.Get($"{serverNode.HttpAddress}/api/info", It.IsAny<TimeSpan>(), token))
                .Returns(new HttpResponseMessage(HttpStatusCode.OK));

            // Act
            var response = _consensusService.ConnectNode(serverNode) as SuccessResponse<ServerNode>;
            await queueTask(token);

            // Assert
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()));
            _httpServiceMock.Verify(p => p.Get($"{serverNode.HttpAddress}/api/info", It.IsAny<TimeSpan>(), token));

            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.Equal("The node has been added successfully!", response.Message);
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
        public void DisconnectFromNetwork_Empty_SuccessResponse()
        {
            // Arrange
            _consensusService.ConnectNode(new ServerNode { Id = "1", HttpAddress = "http://test:4200" });
            _consensusService.ConnectNode(new ServerNode { Id = "2", HttpAddress = "http://test:4200" });

            // Act
            var result = _consensusService.DisconnectFromNetwork() as SuccessResponse<List<ServerNode>>;

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Message);
            Assert.NotNull(result.Result);
            Assert.Equal(2, result.Result.Count);
            Assert.Equal("All nodes has been disconnected!", result.Message);
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
            _consensusService.ConnectNode(new ServerNode { Id = id, HttpAddress = "http://test:4200" });

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
        public void GetNodes_Empty_SuccessResponseWithNodes()
        {
            // Arrange
            _consensusService.ConnectNode(new ServerNode { Id = "1", HttpAddress = "https://test:4200", Delay = 100 });
            _consensusService.ConnectNode(new ServerNode { Id = "2", HttpAddress = "https://test:4200", Delay = 1000 });

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
    }
}