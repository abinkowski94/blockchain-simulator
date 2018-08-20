using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.WebApi.Controllers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BlockchainSimulator.WebApi.Tests.Controllers
{
    public class InfoControllerTests
    {
        private readonly InfoController _infoController;

        public InfoControllerTests()
        {
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("Config", null),
                    new KeyValuePair<string, string>("Config:Testing", "testingValue"),
                    new KeyValuePair<string, string>("Config:Test", "testValue")
                }).Build();
            _infoController = new InfoController(configuration);
        }

        [Fact]
        public void GetInfo_Empty_Configuration()
        {
            // Arrange

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