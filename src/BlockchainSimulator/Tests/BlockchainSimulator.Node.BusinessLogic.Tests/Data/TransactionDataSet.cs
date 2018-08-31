using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using System.Collections.Generic;
using Xunit;

namespace BlockchainSimulator.Node.BusinessLogic.Tests.Data
{
    public class TransactionDataSet
    {
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
                    "c75549000a3044ba91bed6b3df2be50b8247f4ca8482e5d7156e7a3e7cfe3c9b"
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
                    "0c989b6ea19b2391d8c59aeb41b326aa87f7c77b7e4d1994011a389998fd5fc9"
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
                    "3dda268f68fd57368c4c8a72ab5fa98fd85822789b422a53e47ef9a806adf0af"
                },
    };
    }
}