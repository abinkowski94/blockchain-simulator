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
                    "8d6700faac589cb0006df093cd5db8cf496451c90232c939106aaa1a6db5aa89"
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
                    "b8924079106fac6e2fa5588837b75a3502ff977271a8bfd7cf32dd19e12ddbc5"
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
                    "da9e4ab040c871393429f7311263489d3ae62c70886a8c060fbce5ced276b064"
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