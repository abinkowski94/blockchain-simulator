using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Services;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Providers
{
    public class MerkleTreeProviderTests
    {
        private readonly MerkleTreeProvider _merkleTreeProvider;

        public MerkleTreeProviderTests()
        {
            _merkleTreeProvider = new MerkleTreeProvider(new EncryptionService());
        }

        [Fact]
        public void GetMerkleTree_Null_Null()
        {
            // Arrange

            // Act
            var result = _merkleTreeProvider.GetMerkleTree(null);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetMerkleTree_EmptyList_Null()
        {
            // Arrange
            var transactions = new HashSet<Transaction>();

            // Act
            var result = _merkleTreeProvider.GetMerkleTree(transactions);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [MemberData(nameof(Data))]
        public void GetMerkleTree_SetOfTransactions_CorrectMerkleTree(HashSet<Transaction> transactions,
            string resultHash)
        {
            // Arrange

            // Act
            var result = _merkleTreeProvider.GetMerkleTree(transactions);
            
            // Assert
            Assert.NotNull(result);
            Assert.Equal(resultHash, result.Hash);
        }

        public static TheoryData<HashSet<Transaction>, string> Data => new TheoryData<HashSet<Transaction>, string>
        {
            {
                new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = "1",
                        Sender = "00000000",
                        Recipient = "11111111",
                        Amount = 1000,
                        Fee = 0,
                        TransactionJson =
                            "{\"id\": \"1\", \"Sender\": \"00000000\", \"Recipient\": 11111111, \"Amount\" = 1000, \"Fee\" = 0}"
                    }
                },
                "b0e7e9a9000ed690ce9840216710d5b752b73dc1d87368d152edf03c14533c46"
            },
            {
                new HashSet<Transaction>
                {
                    new Transaction
                    {
                        Id = "1",
                        Sender = "00000000",
                        Recipient = "11111111",
                        Amount = 1000,
                        Fee = 0,
                        TransactionJson =
                            "{\"id\": \"1\", \"Sender\": \"00000000\", \"Recipient\": 11111111, \"Amount\" = 1000, \"Fee\" = 0}"
                    },
                    new Transaction
                    {
                        Id = "2",
                        Sender = "00000000",
                        Recipient = "22222222",
                        Amount = 1000,
                        Fee = 0,
                        TransactionJson =
                            "{\"id\": \"2\", \"Sender\": \"00000000\", \"Recipient\": 22222222, \"Amount\" = 1000, \"Fee\" = 0}"
                    },
                    new Transaction
                    {
                        Id = "3",
                        Sender = "11111111",
                        Recipient = "33333333",
                        Amount = 20,
                        Fee = 0.1m,
                        TransactionJson =
                            "{\"id\": \"3\", \"Sender\": \"11111111\", \"Recipient\": 33333333, \"Amount\" = 20, \"Fee\" = 0.1}"
                    },
                    new Transaction
                    {
                        Id = "4",
                        Sender = "22222222",
                        Recipient = "33333333",
                        Amount = 15,
                        Fee = 0.2m,
                        TransactionJson =
                            "{\"id\": \"4\", \"Sender\": \"22222222\", \"Recipient\": 33333333, \"Amount\" = 15, \"Fee\" = 0.2}"
                    },
                    new Transaction
                    {
                        Id = "5",
                        Sender = "33333333",
                        Recipient = "11111111",
                        Amount = 3.1m,
                        Fee = 0.05m,
                        TransactionJson =
                            "{\"id\": \"1\", \"Sender\": \"33333333\", \"Recipient\": 11111111, \"Amount\" = 3.1, \"Fee\" = 0.05}"
                    }
                },
                "77c68adb67e3d365919672221e31c6e8ab2f3ca4310674ab87688f9d9a7f6ae3"
            }
        };
    }
}