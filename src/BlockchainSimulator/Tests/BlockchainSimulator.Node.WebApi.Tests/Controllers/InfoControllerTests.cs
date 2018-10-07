using System.Collections.Generic;
using BlockchainSimulator.Node.WebApi.Controllers;
using System.Linq;
using BlockchainSimulator.Node.BusinessLogic.Services;
using Moq;
using Xunit;

namespace BlockchainSimulator.Node.WebApi.Tests.Controllers
{
    public class InfoControllerTests
    {
        private readonly InfoController _infoController;
        private readonly Mock<IConfigurationService> _configurationServiceMock;

        public InfoControllerTests()
        {
            _configurationServiceMock = new Mock<IConfigurationService>();
            _infoController = new InfoController(_configurationServiceMock.Object);
        }

        [Fact]
        public void GetInfo_Empty_Configuration()
        {
            // Arrange
            _configurationServiceMock.Setup(p => p.GetConfigurationInfo())
                .Returns(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Config:Testing", "testingValue"),
                    new KeyValuePair<string, string>("Config:Test", "testValue")
                });

            // Act
            var result = _infoController.GetInfo();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal("Config:Testing", result.First().Key);
            Assert.Equal("testingValue", result.First().Value);
            Assert.Equal("Config:Test", result.Last().Key);
            Assert.Equal("testValue", result.Last().Value);
        }
    }
}