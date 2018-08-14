using BlockchainSimulator.BusinessLogic.Services;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Services
{
    public class EncryptionServiceTests
    {
        [Fact]
        public void GetSha256Hash_Null_Null()
        {
            // Arrange

            // Act
            var result = EncryptionService.GetSha256Hash(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetSha256Hash_StringEmpty_CorrectHash()
        {
            // Arrange
            const string value = "";

            // Act
            var result = EncryptionService.GetSha256Hash(value);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855", result);
        }

        [Fact]
        public void GetSha256Hash_SameValue_SameHash()
        {
            // Arrange
            const string value = "{ \"testProperty\": \"test data\" }";

            // Act
            var resultOne = EncryptionService.GetSha256Hash(value);
            var resultTwo = EncryptionService.GetSha256Hash(value);

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
            var resultOne = EncryptionService.GetSha256Hash(value);
            var resultTwo = EncryptionService.GetSha256Hash(value);

            // Assert
            Assert.NotNull(resultOne);
            Assert.NotNull(resultTwo);
            Assert.Equal(resultOne, resultTwo);
        }

        [Theory]
        [InlineData("{ \"testProperty\": \"test data\" }")]
        [InlineData("{ \"testProperty\": \"more test\" }")]
        [InlineData("{ \"testProperty\": \"1231\" }")]
        public void GetSha256Hash_ExampleJson_CorrectSha256Hash(string json)
        {
            // Arrange

            // Act
            var result = EncryptionService.GetSha256Hash(json);

            // Assert
            Assert.NotNull(result);
        }
    }
}