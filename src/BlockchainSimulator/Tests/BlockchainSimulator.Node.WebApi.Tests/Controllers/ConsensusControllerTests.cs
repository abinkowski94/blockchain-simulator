using BlockchainSimulator.Common.Models.Consensus;
using BlockchainSimulator.Common.Models.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.WebApi.Controllers;
using BlockchainSimulator.Node.WebApi.Models.Blockchain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BlockchainSimulator.Node.WebApi.Tests.Controllers
{
    public class ConsensusControllerTests
    {
        private readonly ConsensusController _consensusController;
        private readonly Mock<IConsensusService> _consensusServiceMock;

        public ConsensusControllerTests()
        {
            _consensusServiceMock = new Mock<IConsensusService>();
            _consensusController = new ConsensusController(_consensusServiceMock.Object);
        }

        [Fact]
        public void AcceptBlockchain_EncodedBlockchain_True()
        {
            // Arrange
            var encodedBlockchain = new EncodedBlock { Base64Block = "base64Mock" };

            _consensusServiceMock.Setup(p => p.AcceptBlock(encodedBlockchain.Base64Block))
                .Returns(new SuccessResponse<bool>("BlockchainTree has been accepted", true));

            // Act
            var result = _consensusController.AcceptBlockchain(encodedBlockchain);
            var response = (result?.Result as ObjectResult)?.Value as BaseResponse;

            // Assert
            _consensusServiceMock.Verify(p => p.AcceptBlock(encodedBlockchain.Base64Block));

            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.Equal("BlockchainTree has been accepted", response.Message);
            Assert.True(response.Result is bool b && b);
        }

        [Fact]
        public void ConnectNode_Node_Node()
        {
            // Arrange
            var node = new ServerNode { HttpAddress = "http://test:5000", Delay = 100 };

            _consensusServiceMock.Setup(p => p.ConnectNode(It.IsAny<BusinessLogic.Model.Consensus.ServerNode>()))
                .Returns((BusinessLogic.Model.Consensus.ServerNode resultNode) =>
                    new SuccessResponse<BusinessLogic.Model.Consensus.ServerNode>("The node has been added",
                        resultNode));

            // Act
            var result = _consensusController.ConnectNode(node);
            var response = (result?.Result as ObjectResult)?.Value as BaseResponse;
            var responseNode = response?.Result as ServerNode;

            // Assert
            _consensusServiceMock.Verify(p => p.ConnectNode(It.IsAny<BusinessLogic.Model.Consensus.ServerNode>()));

            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.NotNull(responseNode);
            Assert.Equal("The node has been added", response.Message);
            Assert.Equal(node.HttpAddress, responseNode.HttpAddress);
            Assert.Equal(node.Delay, responseNode.Delay);
        }

        [Fact]
        public void DisconnectFromNetwork_Empty_NodesList()
        {
            // Arrange
            _consensusServiceMock.Setup(p => p.DisconnectFromNetwork())
                .Returns(new SuccessResponse<List<BusinessLogic.Model.Consensus.ServerNode>>("The nodes",
                    new List<BusinessLogic.Model.Consensus.ServerNode>
                    {
                        new BusinessLogic.Model.Consensus.ServerNode
                            {Id = "1", HttpAddress = "http://test:5000", Delay = 100, IsConnected = true},
                        new BusinessLogic.Model.Consensus.ServerNode
                            {Id = "2", HttpAddress = "http://test:5001", Delay = 10, IsConnected = false}
                    }));

            // Act
            var result = _consensusController.DisconnectFromNetwork();
            var response = (result?.Result as ObjectResult)?.Value as BaseResponse;
            var nodes = response?.Result as List<ServerNode>;

            // Assert
            _consensusServiceMock.Verify(p => p.DisconnectFromNetwork());

            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.NotNull(nodes);
            Assert.NotEmpty(nodes);
            Assert.Equal(2, nodes.Count);
            Assert.Equal("1", nodes.First().Id);
            Assert.Equal("http://test:5000", nodes.First().HttpAddress);
            Assert.Equal(100, nodes.First().Delay);
            Assert.True(nodes.First().IsConnected);
            Assert.Equal("2", nodes.Last().Id);
            Assert.Equal("http://test:5001", nodes.Last().HttpAddress);
            Assert.Equal(10, nodes.Last().Delay);
            Assert.False(nodes.Last().IsConnected);
        }

        [Fact]
        public void DisconnectNode_Id_Node()
        {
            // Arrange
            const string id = "1";

            _consensusServiceMock.Setup(p => p.DisconnectNode(id))
                .Returns(new SuccessResponse<BusinessLogic.Model.Consensus.ServerNode>("The node has been disconnected",
                    new BusinessLogic.Model.Consensus.ServerNode { Id = id, HttpAddress = "http://test:5999" }));

            // Act
            var result = _consensusController.DisconnectNode(id);
            var response = (result?.Result as ObjectResult)?.Value as BaseResponse;
            var node = response?.Result as ServerNode;

            // Assert
            _consensusServiceMock.Verify(p => p.DisconnectNode(id));

            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.NotNull(node);
            Assert.Equal("The node has been disconnected", response.Message);
            Assert.Equal(id, node.Id);
            Assert.Equal("http://test:5999", node.HttpAddress);
        }

        [Fact]
        public void GetNodes_Empty_NodesList()
        {
            // Arrange
            _consensusServiceMock.Setup(p => p.GetNodes())
                .Returns(new SuccessResponse<List<BusinessLogic.Model.Consensus.ServerNode>>("The nodes",
                    new List<BusinessLogic.Model.Consensus.ServerNode>
                    {
                        new BusinessLogic.Model.Consensus.ServerNode
                            {Id = "1", HttpAddress = "http://test:5000", Delay = 100, IsConnected = true},
                        new BusinessLogic.Model.Consensus.ServerNode
                            {Id = "2", HttpAddress = "http://test:5001", Delay = 10, IsConnected = false}
                    }));

            // Act
            var result = _consensusController.GetNodes();
            var response = (result?.Result as ObjectResult)?.Value as BaseResponse;
            var nodes = response?.Result as List<ServerNode>;

            // Assert
            _consensusServiceMock.Verify(p => p.GetNodes());

            Assert.NotNull(result);
            Assert.NotNull(response);
            Assert.NotNull(nodes);
            Assert.NotEmpty(nodes);
            Assert.Equal(2, nodes.Count);
            Assert.Equal("1", nodes.First().Id);
            Assert.Equal("http://test:5000", nodes.First().HttpAddress);
            Assert.Equal(100, nodes.First().Delay);
            Assert.True(nodes.First().IsConnected);
            Assert.Equal("2", nodes.Last().Id);
            Assert.Equal("http://test:5001", nodes.Last().HttpAddress);
            Assert.Equal(10, nodes.Last().Delay);
            Assert.False(nodes.Last().IsConnected);
        }
    }
}