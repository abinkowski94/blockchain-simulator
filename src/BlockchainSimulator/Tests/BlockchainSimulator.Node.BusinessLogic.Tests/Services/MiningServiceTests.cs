using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Tests.Data;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Services
{
    public class MiningServiceTests
    {
        private readonly Mock<IBlockchainRepository> _blockchainRepositoryMock;
        private readonly Mock<IBlockProvider> _blockProviderMock;
        private readonly Mock<IConsensusService> _consensusServiceMock;
        private readonly Mock<IMiningQueue> _miningQueueMock;
        private readonly MiningService _miningService;

        public MiningServiceTests()
        {
            _blockchainRepositoryMock = new Mock<IBlockchainRepository>();
            _blockProviderMock = new Mock<IBlockProvider>();
            _consensusServiceMock = new Mock<IConsensusService>();
            _miningQueueMock = new Mock<IMiningQueue>();
            _miningService = new MiningService(_blockchainRepositoryMock.Object, _consensusServiceMock.Object, new Mock<IStatisticService>().Object, _blockProviderMock.Object);
        }
    }
}