using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Common.Services;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Consensus;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;
using BlockchainSimulator.Node.BusinessLogic.Services;
using BlockchainSimulator.Node.BusinessLogic.Services.Specific;
using BlockchainSimulator.Node.BusinessLogic.Validators;
using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Moq;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

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

            _consensusService = new ProofOfWorkConsensusService(_backgroundTaskQueueMock.Object,
                _blockchainRepositoryMock.Object, _blockchainValidatorMock.Object, httpServiceMock.Object, new Mock<IStatisticService>().Object,
                new Mock<IServiceProvider>().Object);
        }

        [Fact]
        public void AcceptBlockchain_Encoded_ErrorResponseInvalid()
        {
            // Arrange
            const string blockchainJson =
                "{\"blocks\":[{\"isGenesis\":true,\"id\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":null,\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"sender\":\"000000000000000000000000000000000000000000000000000000000000000\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"amount\":1000.0,\"fee\":0.0}],\"transactionCounter\":1},\"header\":{\"version\":\"1\",\"parentHash\":null,\"merkleTreeRootHash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"27288b6b4a31d141aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}},{\"parentId\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"isGenesis\":false,\"id\":\"2ab6c4de-d991-4c7e-af71-de385deb73cb\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"hash\":\"1131f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":{\"transactionId\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"hash\":\"2231f911d761cd47834b3fa6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"hash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAftftrRRzQ9qg4k6528UexqpxjCLXd++OkzruIBY1RYRT8wThK3/bn4fgWCCCND/Rbgth3cO7OQt448R7yOoEPwIDAQAB\",\"amount\":21.0,\"fee\":1.0},{\"id\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKJ7lyKUiqKEDouTGJomrRWnVa7B1/Zd7+GIqFU50WeJF3jrfkgNqTF6dJou9xRJPOPBKBv2LJiCIHhrD8EXdYcCAwEAAQ==\",\"amount\":10.0,\"fee\":0.1}],\"transactionCounter\":2},\"header\":{\"version\":\"1\",\"parentHash\":\"312388b6b4a31d141a412312211c1da8a838e3a5a5f51a96df1d296055746a0df569\",\"merkleTreeRootHash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"31231231aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}}]}";

            var blockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);
            blockchain.Blocks.RemoveAt(1);

            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(blockchain);

            var inputBlockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);
            var jsonToEncode = JsonConvert.SerializeObject(inputBlockchain);
            var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonToEncode));

            _blockchainValidatorMock.Setup(p => p.Validate(It.IsAny<BlockBase>()))
                .Returns(new ValidationResult(false, "The incoming blockchain is invalid!"));

            // Act
            var result = _consensusService.AcceptBlockchain(encodedBlockchain) as ErrorResponse<bool>;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());
            _blockchainValidatorMock.Verify(p => p.Validate(It.IsAny<BlockBase>()));
            _blockchainRepositoryMock.Verify(p => p.SaveBlockchain(It.IsAny<BlockchainTree>()), Times.Never());
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Never);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Equal("The incoming blockchain is invalid!", result.Message);
            Assert.Single(result.Errors);
        }

        [Fact]
        public void AcceptBlockchain_Encoded_ErrorResponseShorter()
        {
            // Arrange
            const string blockchainJson =
                "{\"blocks\":[{\"isGenesis\":true,\"id\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":null,\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"sender\":\"000000000000000000000000000000000000000000000000000000000000000\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"amount\":1000.0,\"fee\":0.0}],\"transactionCounter\":1},\"header\":{\"version\":\"1\",\"parentHash\":null,\"merkleTreeRootHash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"27288b6b4a31d141aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}},{\"parentId\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"isGenesis\":false,\"id\":\"2ab6c4de-d991-4c7e-af71-de385deb73cb\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"hash\":\"1131f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":{\"transactionId\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"hash\":\"2231f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"hash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAftftrRRzQ9qg4k6528UexqpxjCLXd++OkzruIBY1RYRT8wThK3/bn4fgWCCCND/Rbgth3cO7OQt448R7yOoEPwIDAQAB\",\"amount\":21.0,\"fee\":1.0},{\"id\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKJ7lyKUiqKEDouTGJomrRWnVa7B1/Zd7+GIqFU50WeJF3jrfkgNqTF6dJou9xRJPOPBKBv2LJiCIHhrD8EXdYcCAwEAAQ==\",\"amount\":10.0,\"fee\":0.1}],\"transactionCounter\":2},\"header\":{\"version\":\"1\",\"parentHash\":\"312388b6b4a31d141a412312211c1da8a838e3a5a5f51a96df1d296055746a0df569\",\"merkleTreeRootHash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"31231231aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}}]}";

            var blockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);

            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(blockchain);

            var inputBlockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);
            inputBlockchain.Blocks.RemoveAt(1);
            var jsonToEncode = JsonConvert.SerializeObject(inputBlockchain);
            var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonToEncode));

            // Act
            var result = _consensusService.AcceptBlockchain(encodedBlockchain) as ErrorResponse<bool>;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());
            _blockchainValidatorMock.Verify(p => p.Validate(It.IsAny<BlockBase>()), Times.Never);
            _blockchainRepositoryMock.Verify(p => p.SaveBlockchain(It.IsAny<BlockchainTree>()), Times.Never());
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Never);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Equal("The incoming blockchain is shorter than the current!", result.Message);
        }

        [Fact]
        public void AcceptBlockchain_Encoded_SuccessResponse()
        {
            // Arrange
            const string blockchainJson =
                "{\"blocks\":[{\"isGenesis\":true,\"id\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":null,\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"sender\":\"000000000000000000000000000000000000000000000000000000000000000\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"amount\":1000.0,\"fee\":0.0}],\"transactionCounter\":1},\"header\":{\"version\":\"1\",\"parentHash\":null,\"merkleTreeRootHash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"27288b6b4a31d141aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}},{\"parentId\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"isGenesis\":false,\"id\":\"2ab6c4de-d991-4c7e-af71-de385deb73cb\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"hash\":\"1131f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":{\"transactionId\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"hash\":\"2231f911d761cd47834b3fa6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"hash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAftftrRRzQ9qg4k6528UexqpxjCLXd++OkzruIBY1RYRT8wThK3/bn4fgWCCCND/Rbgth3cO7OQt448R7yOoEPwIDAQAB\",\"amount\":21.0,\"fee\":1.0},{\"id\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKJ7lyKUiqKEDouTGJomrRWnVa7B1/Zd7+GIqFU50WeJF3jrfkgNqTF6dJou9xRJPOPBKBv2LJiCIHhrD8EXdYcCAwEAAQ==\",\"amount\":10.0,\"fee\":0.1}],\"transactionCounter\":2},\"header\":{\"version\":\"1\",\"parentHash\":\"312388b6b4a31d141a412312211c1da8a838e3a5a5f51a96df1d296055746a0df569\",\"merkleTreeRootHash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"31231231aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}}]}";

            var blockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);
            blockchain.Blocks.RemoveAt(1);

            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(blockchain);

            var inputBlockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);
            var jsonToEncode = JsonConvert.SerializeObject(inputBlockchain);
            var encodedBlockchain = Convert.ToBase64String(Encoding.UTF8.GetBytes(jsonToEncode));

            _blockchainValidatorMock.Setup(p => p.Validate(It.IsAny<BlockBase>()))
                .Returns(new ValidationResult(true));

            // Act
            var result = _consensusService.AcceptBlockchain(encodedBlockchain) as SuccessResponse<bool>;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());
            _blockchainValidatorMock.Verify(p => p.Validate(It.IsAny<BlockBase>()));
            _blockchainRepositoryMock.Verify(p => p.SaveBlockchain(It.IsAny<BlockchainTree>()));
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Never);

            Assert.NotNull(result);
            Assert.True(result.Result);
            Assert.Equal("The blockchain has been accepted and swapped!", result.Message);
        }

        [Fact]
        public void AcceptBlockchain_Null_ErrorResponse()
        {
            // Arrange

            // Act
            var result = _consensusService.AcceptBlockchain((string) null) as ErrorResponse<bool>;

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree(), Times.Never());
            _blockchainValidatorMock.Verify(p => p.Validate(It.IsAny<BlockBase>()), Times.Never);
            _blockchainRepositoryMock.Verify(p => p.SaveBlockchain(It.IsAny<BlockchainTree>()), Times.Never());
            _backgroundTaskQueueMock.Verify(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()),
                Times.Never);

            Assert.NotNull(result);
            Assert.False(result.Result);
            Assert.Equal("The blockchain can not be null!", result.Message);
        }

        [Fact]
        public async Task ReachConsensus_Empty_Void()
        {
            // Arrange
            _consensusService.ConnectNode(new ServerNode {Id = "1", HttpAddress = "https://test:4200"});
            _consensusService.ConnectNode(new ServerNode {Id = "2", HttpAddress = "https://test:4200"});

            var token = new CancellationToken();
            Func<CancellationToken, Task> queueTask = null;
            _backgroundTaskQueueMock.Setup(p => p.QueueBackgroundWorkItem(It.IsAny<Func<CancellationToken, Task>>()))
                .Callback((Func<CancellationToken, Task> func) => queueTask = func);

            const string blockchainJson =
                "{\"blocks\":[{\"isGenesis\":true,\"id\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":null,\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"sender\":\"000000000000000000000000000000000000000000000000000000000000000\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"amount\":1000.0,\"fee\":0.0}],\"transactionCounter\":1},\"header\":{\"version\":\"1\",\"parentHash\":null,\"merkleTreeRootHash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"27288b6b4a31d141aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}},{\"parentId\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"isGenesis\":false,\"id\":\"2ab6c4de-d991-4c7e-af71-de385deb73cb\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"hash\":\"1131f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":{\"transactionId\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"hash\":\"2231f911d761cd47834b3fa6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"hash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAftftrRRzQ9qg4k6528UexqpxjCLXd++OkzruIBY1RYRT8wThK3/bn4fgWCCCND/Rbgth3cO7OQt448R7yOoEPwIDAQAB\",\"amount\":21.0,\"fee\":1.0},{\"id\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuy\",\"recipient\":\"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKJ7lyKUiqKEDouTGJomrRWnVa7B1/Zd7+GIqFU50WeJF3jrfkgNqTF6dJou9xRJPOPBKBv2LJiCIHhrD8EXdYcCAwEAAQ==\",\"amount\":10.0,\"fee\":0.1}],\"transactionCounter\":2},\"header\":{\"version\":\"1\",\"parentHash\":\"312388b6b4a31d141a412312211c1da8a838e3a5a5f51a96df1d296055746a0df569\",\"merkleTreeRootHash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"31231231aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}}]}";

            var blockchain = BlockchainConverter.DeserializeBlockchain(blockchainJson);

            _blockchainRepositoryMock.Setup(p => p.GetBlockchainTree())
                .Returns(blockchain);

            // Act
            _consensusService.ReachConsensus();
            await queueTask(token);

            // Assert
            _blockchainRepositoryMock.Verify(p => p.GetBlockchainTree());
        }
    }
}