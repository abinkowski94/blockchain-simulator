using BlockchainSimulator.BusinessLogic.Services;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Services
{
    public class EncryptionServiceTests
    {
        private readonly EncryptionService _encryptionService;

        public EncryptionServiceTests()
        {
            _encryptionService = new EncryptionService();
        }

        [Theory]
        [InlineData("{ \"testProperty\": \"test data\" }")]
        [InlineData("{ \"testProperty\": \"more test\" }")]
        [InlineData("{ \"testProperty\": \"1231\" }")]
        public void GetSha256Hash_ExampleJson_CorrectSha256Hash(string json)
        {
            // Arrange

            // Act
            var result = _encryptionService.GetSha256Hash(json);

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void GetSha256Hash_SameValue_SameHash()
        {
            // Arrange
            const string value = "{ \"testProperty\": \"test data\" }";

            // Act
            var resultOne = _encryptionService.GetSha256Hash(value);
            var resultTwo = _encryptionService.GetSha256Hash(value);

            // Assert
            Assert.NotNull(resultOne);
            Assert.NotNull(resultTwo);
            Assert.Equal(resultOne, resultTwo);
        }
        
        [Fact]
        public void GetSha256Hash_DifferentValue_DifferentHash()
        {
            // Arrange
            const string value = "{ \"testProperty\": \"test data 1\" }";

            // Act
            var resultOne = _encryptionService.GetSha256Hash(value);
            var resultTwo = _encryptionService.GetSha256Hash(value);

            // Assert
            Assert.NotNull(resultOne);
            Assert.NotNull(resultTwo);
            Assert.Equal(resultOne, resultTwo);
        }
    }
}