using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using Xunit;

namespace BlockchainSimulator.BusinessLogic.Tests.Data
{
    public class TransactionDataSet
    {
        public static TheoryData<HashSet<Transaction>, string> TransactionDataAndMerkleTreeHashResults =>
            new TheoryData<HashSet<Transaction>, string>
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
                            Fee = 0
                        }
                    },
                    "075e2ee940f291540722d8c3ea2433a9e4806a8b4dfb3a58d7fd338774e0bb66"
                },
                {
                    new HashSet<Transaction>
                    {
                        new Transaction
                        {
                            Id = "1",
                            Sender = "11111111",
                            Recipient = "33333333",
                            Amount = 11,
                            Fee = 0.5m
                        },
                        new Transaction
                        {
                            Id = "2",
                            Sender = "11111111",
                            Recipient = "22222222",
                            Amount = 12,
                            Fee = 0.1m
                        },
                        new Transaction
                        {
                            Id = "3",
                            Sender = "11111111",
                            Recipient = "44444444",
                            Amount = 13.1m,
                            Fee = 0.65m
                        }
                    },
                    "b6a350ceb8531ecb0c087a94f0abdf017543c5f65bd71c893e21a77a5d31b53b"
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
                            Fee = 0
                        },
                        new Transaction
                        {
                            Id = "2",
                            Sender = "00000000",
                            Recipient = "22222222",
                            Amount = 1000,
                            Fee = 0
                        },
                        new Transaction
                        {
                            Id = "3",
                            Sender = "11111111",
                            Recipient = "33333333",
                            Amount = 20,
                            Fee = 0.1m
                        },
                        new Transaction
                        {
                            Id = "4",
                            Sender = "22222222",
                            Recipient = "33333333",
                            Amount = 15,
                            Fee = 0.2m
                        },
                        new Transaction
                        {
                            Id = "5",
                            Sender = "33333333",
                            Recipient = "11111111",
                            Amount = 3.1m,
                            Fee = 0.05m
                        }
                    },
                    "527f2414cd36a489c11d018f71dad8ba609ae1a7781a103c1ba5bf249ac5de87"
                },
            };

        public static TheoryData<HashSet<Transaction>> TransactionData => new TheoryData<HashSet<Transaction>>
        {
            new HashSet<Transaction>
            {
                new Transaction
                {
                    Id = "1",
                    Sender = "00000000",
                    Recipient = "11111111",
                    Amount = 1000,
                    Fee = 0
                }
            },
            new HashSet<Transaction>
            {
                new Transaction
                {
                    Id = "1",
                    Sender = "11111111",
                    Recipient = "33333333",
                    Amount = 11,
                    Fee = 0.5m
                },
                new Transaction
                {
                    Id = "2",
                    Sender = "11111111",
                    Recipient = "22222222",
                    Amount = 12,
                    Fee = 0.1m
                },
                new Transaction
                {
                    Id = "3",
                    Sender = "11111111",
                    Recipient = "44444444",
                    Amount = 13.1m,
                    Fee = 0.65m
                }
            },
            new HashSet<Transaction>
            {
                new Transaction
                {
                    Id = "1",
                    Sender = "00000000",
                    Recipient = "11111111",
                    Amount = 1000,
                    Fee = 0
                },
                new Transaction
                {
                    Id = "2",
                    Sender = "00000000",
                    Recipient = "22222222",
                    Amount = 1000,
                    Fee = 0
                },
                new Transaction
                {
                    Id = "3",
                    Sender = "11111111",
                    Recipient = "33333333",
                    Amount = 20,
                    Fee = 0.1m
                },
                new Transaction
                {
                    Id = "4",
                    Sender = "22222222",
                    Recipient = "33333333",
                    Amount = 15,
                    Fee = 0.2m
                },
                new Transaction
                {
                    Id = "5",
                    Sender = "33333333",
                    Recipient = "11111111",
                    Amount = 3.1m,
                    Fee = 0.05m
                }
            }
        };
    }
}