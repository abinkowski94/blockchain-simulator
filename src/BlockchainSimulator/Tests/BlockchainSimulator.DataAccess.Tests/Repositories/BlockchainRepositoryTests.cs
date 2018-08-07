using System;
using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.DataAccess.Model;
using BlockchainSimulator.DataAccess.Model.Block;
using BlockchainSimulator.DataAccess.Model.Transaction;
using BlockchainSimulator.DataAccess.Repositories;
using Moq;
using Xunit;

namespace BlockchainSimulator.DataAccess.Tests.Repositories
{
    public class BlockchainRepositoryTests
    {
        private readonly Mock<IFileRepository> _fileRepositoryMock;
        private readonly BlockchainRepository _blockchainRepository;

        public BlockchainRepositoryTests()
        {
            _fileRepositoryMock = new Mock<IFileRepository>();
            _blockchainRepository = new BlockchainRepository(_fileRepositoryMock.Object);
        }

        [Fact]
        public void SaveBlockchain_SaveExampleBlockchain_SavedBlockchain()
        {
            // Arrange
            var genesisBlockTransactionOne = new Transaction(Guid.NewGuid().ToString(),
                @"00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                @"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuyJIh6LyU9UXNJg73j1YB/sYHvm07fP3cCixmSboHJru3ycE4yjZAednMxQIDAQAB",
                1000, 0);
            var genesisBlockTransactions = new HashSet<Transaction> {genesisBlockTransactionOne};
            var genesisBlockMerkleTreeLeaf = new Leaf(
                "9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b",
                genesisBlockTransactionOne.Id);
            var genesisBlockMerkleTree = new Node(genesisBlockMerkleTreeLeaf.Hash, genesisBlockMerkleTreeLeaf, null);
            var genesisBlockBody = new Body(genesisBlockTransactions, genesisBlockMerkleTree);
            var genesisBlockHeader = new Header("1", null, genesisBlockMerkleTree.Hash, DateTime.Now, "0000",
                "27288b6b4a31d141aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569");
            var genesisBlock = new GenesisBlock(genesisBlockBody, genesisBlockHeader, Guid.NewGuid().ToString());

            var blockTransactionOne = new Transaction(Guid.NewGuid().ToString(),
                @"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuyJIh6LyU9UXNJg73j1YB/sYHvm07fP3cCixmSboHJru3ycE4yjZAednMxQIDAQAB",
                @"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAftftrRRzQ9qg4k6528UexqpxjCLXd++OkzruIBY1RYRT8wThK3/bn4fgWCCCND/Rbgth3cO7OQt448R7yOoEPwIDAQAB",
                21, 1);
            var blockTransactionTwo = new Transaction(Guid.NewGuid().ToString(),
                @"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuyJIh6LyU9UXNJg73j1YB/sYHvm07fP3cCixmSboHJru3ycE4yjZAednMxQIDAQAB",
                @"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKJ7lyKUiqKEDouTGJomrRWnVa7B1/Zd7+GIqFU50WeJF3jrfkgNqTF6dJou9xRJPOPBKBv2LJiCIHhrD8EXdYcCAwEAAQ==",
                10, 0.1m);
            var blockTransactions = new HashSet<Transaction> {blockTransactionOne, blockTransactionTwo};
            var blockMerkleTreeLeafOne = new Leaf("1131f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b",
                blockTransactionOne.Id);
            var blockMerkleTreeLeafTwo = new Leaf("2231f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b",
                blockTransactionTwo.Id);
            var blockMerkleTree = new Node("3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b",
                blockMerkleTreeLeafOne, blockMerkleTreeLeafTwo);
            var blockBody = new Body(blockTransactions, blockMerkleTree);
            var blockHeader = new Header("1", "312388b6b4a31d141a412312211c1da8a838e3a5a5f51a96df1d296055746a0df569",
                blockMerkleTree.Hash, DateTime.Now, "0000", "31231231aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569");
            var block = new Block(blockBody, blockHeader, Guid.NewGuid().ToString(), genesisBlock.Id);

            var blockchain = new Blockchain
            {
                Blocks = {genesisBlock, block}
            };

            // Act
            var result = _blockchainRepository.SaveBlockchain(blockchain);

            // Assert
            _fileRepositoryMock.Verify(p => p.SaveFile(It.IsAny<string>(), "blockchain.json"));

            Assert.Equal(blockchain, result);
        }

        [Fact]
        public void GetBlockchain_JsonValidBlockchain_ValidBlockchain()
        {
            // Arrange
            const string blockchainJson =
                "{\"blocks\":[{\"isGenesis\":true,\"id\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":null,\"hash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"7153a819-560e-4218-a5c4-6a2b3b784307\",\"sender\":\"00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuyJIh6LyU9UXNJg73j1YB/sYHvm07fP3cCixmSboHJru3ycE4yjZAednMxQIDAQAB\",\"amount\":1000.0,\"fee\":0.0}],\"transactionCounter\":1},\"header\":{\"version\":\"1\",\"parentHash\":null,\"merkleTreeRootHash\":\"9531f911d761cd47834b3fc6e21ee053b09bd376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"27288b6b4a31d141aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}},{\"parentId\":\"dc9fa87c-5419-4c80-a318-54bb85ca9fa5\",\"isGenesis\":false,\"id\":\"2ab6c4de-d991-4c7e-af71-de385deb73cb\",\"body\":{\"merkleTree\":{\"leftNode\":{\"transactionId\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"hash\":\"1131f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"rightNode\":{\"transactionId\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"hash\":\"2231f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"hash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\"},\"transactions\":[{\"id\":\"62cf99a5-568d-4255-b1f3-694e8c712cfd\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuyJIh6LyU9UXNJg73j1YB/sYHvm07fP3cCixmSboHJru3ycE4yjZAednMxQIDAQAB\",\"recipient\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAftftrRRzQ9qg4k6528UexqpxjCLXd++OkzruIBY1RYRT8wThK3/bn4fgWCCCND/Rbgth3cO7OQt448R7yOoEPwIDAQAB\",\"amount\":21.0,\"fee\":1.0},{\"id\":\"07e405bf-ebcd-48d7-87f5-695eeee09e8b\",\"sender\":\"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuyJIh6LyU9UXNJg73j1YB/sYHvm07fP3cCixmSboHJru3ycE4yjZAednMxQIDAQAB\",\"recipient\":\"MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBAKJ7lyKUiqKEDouTGJomrRWnVa7B1/Zd7+GIqFU50WeJF3jrfkgNqTF6dJou9xRJPOPBKBv2LJiCIHhrD8EXdYcCAwEAAQ==\",\"amount\":10.0,\"fee\":0.1}],\"transactionCounter\":2},\"header\":{\"version\":\"1\",\"parentHash\":\"312388b6b4a31d141a412312211c1da8a838e3a5a5f51a96df1d296055746a0df569\",\"merkleTreeRootHash\":\"3331f911d761cd47834b3fc6e21ee053b0123376b54a5c7ff3bdfc3558c7820b\",\"timeStamp\":\"2018-08-06T15:43:20.8218729+02:00\",\"target\":\"0000\",\"nonce\":\"31231231aeae211c1da8a838e3a5a5f51a96df1d296055746a0df569\"}}]}";

            _fileRepositoryMock.Setup(p => p.GetFile("blockchain.json"))
                .Returns(blockchainJson);

            // Act
            var result = _blockchainRepository.GetBlockchain();

            // Assert
            _fileRepositoryMock.Verify(p => p.GetFile("blockchain.json"));

            Assert.NotNull(result);
            Assert.NotNull(result.Blocks);
            Assert.Equal(2, result.Blocks.Count);
            Assert.True(result.Blocks.First().IsGenesis);
            Assert.Equal("0000", result.Blocks.First().Header.Target);
            Assert.Equal(
                "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000",
                result.Blocks.First().Body.Transactions.First().Sender);
            Assert.False(result.Blocks.Last().IsGenesis);
            Assert.Equal("0000", result.Blocks.Last().Header.Target);
            Assert.Equal(
                @"MFswDQYJKoZIhvcNAQEBBQADSgAwRwJAYnEJ5opsGtKxG6AJ9XxZznKVKcRuyJIh6LyU9UXNJg73j1YB/sYHvm07fP3cCixmSboHJru3ycE4yjZAednMxQIDAQAB",
                result.Blocks.Last().Body.Transactions.First().Sender);
        }
    }
}