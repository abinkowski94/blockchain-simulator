using System.Collections.Generic;
using BlockchainSimulator.WebApi.AppStart;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BlockchainSimulator.WebApi.Tests.AppStart
{
    public class ServicesRegistrationTests
    {
        [Fact]
        public void AddBlockchainServices_ServicesAndConfiguration_Services()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
                {{"Node:Type", "PoW"}}).Build();

            // Act
            var result = services.AddBlockchainServices(configuration);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(15, result.Count);
        }
    }
}