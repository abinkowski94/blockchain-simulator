using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using BlockchainSimulator.Node.DataAccess.Model.Transaction;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace BlockchainSimulator.Node.DataAccess.Tests.Repositories
{
    public class BlockchainRepositoryTests
    {
        private readonly BlockchainRepository _blockchainRepository;
        private readonly Mock<IFileRepository> _fileRepositoryMock;

        public BlockchainRepositoryTests()
        {
            _fileRepositoryMock = new Mock<IFileRepository>();
            _blockchainRepository = new BlockchainRepository(_fileRepositoryMock.Object);
        }
    }
}