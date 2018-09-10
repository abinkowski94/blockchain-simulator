using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IO;
using Xunit;

namespace BlockchainSimulator.Node.DataAccess.Tests.Repositories
{
    public class FileRepositoryTests
    {
        private readonly FileRepository _fileRepository;
        private readonly Mock<IHostingEnvironment> _hostingEnvironmentMock;

        public FileRepositoryTests()
        {
            _hostingEnvironmentMock = new Mock<IHostingEnvironment>();
            _hostingEnvironmentMock.Setup(p => p.ContentRootPath).Returns(Directory.GetCurrentDirectory());
            _fileRepository = new FileRepository(_hostingEnvironmentMock.Object, new Mock<IConfiguration>().Object);
        }
    }
}