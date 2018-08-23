using BlockchainSimulator.Node.BusinessLogic.Configurations;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Configurations
{
    public class ProofOfWorkConfigurationTests
    {
        [Fact]
        public void ProofOfWorkConfigurationCtor_Empty_Object()
        {
            // Arrange
            var configuration = new ConfigurationBuilder().Build();

            // Act
            var result = new ProofOfWorkConfiguration(configuration);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Target);
            Assert.NotNull(result.Version);
            Assert.Equal(10, result.BlockSize);
        }
    }
}