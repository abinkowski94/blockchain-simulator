using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Node.WebApi.AppStart;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BlockchainSimulator.Node.WebApi.Tests.AppStart
{
    public class ConfigurationBuilderExtensionsTests
    {
        [Fact]
        public void AddJsonFiles_BuilderAndPathsAndArgs_Configuration()
        {
            // Arrange
            var builder = new ConfigurationBuilder();

            // Act
            var result = builder.AddJsonFiles(new List<string> {"test.json"}, true, "urls|-|https://localhost:5002",
                "contentRoot|-|wwwroot", "Node:Id|-|2", "Node:Type|-|PoW", "BlockchainConfiguration:Version|-|PoW-v1",
                "BlockchainConfiguration:Target|-|0000", "BlockchainConfiguration:BlockSize|-|5");
            var properties = result.Build().AsEnumerable().ToList();

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(properties);
            Assert.Equal(9, properties.Count);
            Assert.Equal("urls", properties.First().Key);
            Assert.Equal("https://localhost:5002", properties.First().Value);
            Assert.Equal("BlockchainConfiguration:BlockSize", properties.Last().Key);
            Assert.Equal("5", properties.Last().Value);
        }
    }
}