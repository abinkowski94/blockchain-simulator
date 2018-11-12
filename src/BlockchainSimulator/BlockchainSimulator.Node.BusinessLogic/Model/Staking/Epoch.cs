using System.Collections.Concurrent;
using System.Linq;
using BlockchainSimulator.Node.DataAccess.Model.Messages;
using Newtonsoft.Json;
using DAMT = BlockchainSimulator.Node.DataAccess.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Staking
{
    public class Epoch
    {
        public string FinalizedBlockId { get; set; }
        public string PreparedBlockId { get; set; }
        public bool HasPrepared => PreparedBlockId != null;
        public bool HasFinalized => FinalizedBlockId != null;

        public int Number { get; }
        [JsonIgnore] public Epoch PreviousEpoch { get; }
        public ConcurrentDictionary<string, DAMT.Transaction> Transactions { get; }
        public ConcurrentDictionary<string, decimal> CheckpointsWithPrepareStakes { get; }
        public ConcurrentDictionary<string, decimal> CheckpointsWithCommitStakes { get; }

        public Epoch(Epoch previousEpoch, int number)
        {
            CheckpointsWithPrepareStakes = new ConcurrentDictionary<string, decimal>();
            CheckpointsWithCommitStakes = new ConcurrentDictionary<string, decimal>();
            Transactions = new ConcurrentDictionary<string, DAMT.Transaction>();
            PreviousEpoch = previousEpoch;
            Number = number;
        }

        public decimal TotalStake
        {
            get
            {
                var depositedTransactions = Transactions.Values.Where(t =>
                    t.TransactionMessage?.MessageType == TransactionMessageTypes.Deposit).ToList();
                var withdrawTransactions = Transactions.Values.Where(t =>
                    t.TransactionMessage?.MessageType == TransactionMessageTypes.Withdraw).ToList();

                return PreviousEpoch?.TotalStake ?? 0
                       + (depositedTransactions.Any() ? depositedTransactions.Sum(t => t.Amount) : 0)
                       - (withdrawTransactions.Any() ? withdrawTransactions.Sum(t => t.Amount) : 0);
            }
        }

        public decimal GetStakeForValidator(string validatorId)
        {
            var result = 0m;
            var epoch = PreviousEpoch;

            while (epoch != null)
            {
                if (epoch.HasFinalized)
                {
                    var depositedTransactions = epoch.Transactions.Values
                        .Where(t => t.Sender == validatorId &&
                                    t.TransactionMessage?.MessageType == TransactionMessageTypes.Deposit).ToList();
                    var withdrawTransactions = epoch.Transactions.Values
                        .Where(t => t.Sender == validatorId &&
                                    t.TransactionMessage?.MessageType == TransactionMessageTypes.Withdraw).ToList();

                    result += depositedTransactions.Any() ? depositedTransactions.Sum(t => t.Amount) : 0;
                    result -= withdrawTransactions.Any() ? withdrawTransactions.Sum(t => t.Amount) : 0;
                }

                epoch = epoch.PreviousEpoch;
            }

            return result;
        }
    }
}