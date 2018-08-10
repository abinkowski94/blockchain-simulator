using System.IO;
using BlockchainSimulator.DataAccess.Repositories;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Xunit;

namespace BlockchainSimulator.DataAccess.Tests.Repositories
{
    public class FileRepositoryTests
    {
        private readonly FileRepository _fileRepository;
        private readonly Mock<IHostingEnvironment> _hostingEnvironmentMock;

        public FileRepositoryTests()
        {
            _hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            _hostingEnvironmentMock.Setup(p => p.WebRootPath).Returns(Directory.GetCurrentDirectory());
            _fileRepository = new FileRepository(_hostingEnvironmentMock.Object);
        }

        [Fact]
        public void SaveFile_CorrectDataAndFileName_Void()
        {
            // Arrange
            const string data = "{ \"test\": \"aaa\"}";
            const string fileName = "test.json";
            
            // Act
            _fileRepository.SaveFile(data, fileName);
            
            // Assert
            _hostingEnvironmentMock.Verify(p => p.WebRootPath);
        }

        [Fact]
        public void GetFile_CorrectFileName_CorrectData()
        {
            // Arrange
            const string data = "{ \"test\": \"aaa\"}";
            const string fileName = "test.json";
            _fileRepository.SaveFile(data, fileName);

            // Act
            var result = _fileRepository.GetFile(fileName);

            // Assert
            _hostingEnvironmentMock.Verify(p => p.WebRootPath);
            
            Assert.Equal(data, result);
        }
        
        [Fact]
        public void GetFile_WrongFileName_Null()
        {
            // Arrange

            // Act
            var result = _fileRepository.GetFile("not-existing.json");

            // Assert
            _hostingEnvironmentMock.Verify(p => p.WebRootPath);
            
            Assert.Null(result);
        }
    }
}