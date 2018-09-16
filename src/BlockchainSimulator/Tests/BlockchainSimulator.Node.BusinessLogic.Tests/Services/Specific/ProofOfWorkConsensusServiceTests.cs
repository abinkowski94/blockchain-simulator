using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Services.Specific;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Moq;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Services.Specific
{
    public class ProofOfWorkConsensusServiceTests
    {
        private readonly Mock<IBackgroundTaskQueue> _backgroundTaskQueueMock;
        private readonly Mock<IBlockchainRepository> _blockchainRepositoryMock;
        private readonly Mock<IBlockchainValidator> _blockchainValidatorMock;
        private readonly ProofOfWorkConsensusService _consensusService;

        public ProofOfWorkConsensusServiceTests()
        {
            _backgroundTaskQueueMock = new Mock<IBackgroundTaskQueue>();
            _blockchainRepositoryMock = new Mock<IBlockchainRepository>();
            _blockchainValidatorMock = new Mock<IBlockchainValidator>();
            var httpServiceMock = new Mock<IHttpService>();

//            _consensusService = new ProofOfWorkConsensusService(_backgroundTaskQueueMock.Object,
//                _blockchainRepositoryMock.Object, _blockchainValidatorMock.Object, httpServiceMock.Object,
//                new Mock<IStatisticService>().Object);
        }
    }
}