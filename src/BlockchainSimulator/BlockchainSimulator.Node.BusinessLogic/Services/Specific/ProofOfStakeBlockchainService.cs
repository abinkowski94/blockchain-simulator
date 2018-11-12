using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlockchainSimulator.Node.BusinessLogic.Model.Staking;
using BlockchainSimulator.Node.BusinessLogic.Storage;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using BlockchainSimulator.Node.DataAccess.Model.Messages;
using BlockchainSimulator.Node.DataAccess.Model.Transaction;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfStakeBlockchainService : ProofOfWorkBlockchainService
    {
        private const decimal TwoThird = 0.666_666_666_666_666_666_666_666_666M;
        private readonly int _epochSize;
        private readonly bool _isValidator;
        private readonly Dictionary<string, int> _startupValidators;
        private readonly IStakingStorage _stakingStorage;
        private readonly IServiceProvider _serviceProvider;

        public ProofOfStakeBlockchainService(IConfigurationService configurationService,
            IBlockchainRepository blockchainRepository, IStakingStorage stakingStorage,
            IServiceProvider serviceProvider) : base(configurationService, blockchainRepository)
        {
            var configuration = configurationService.GetConfiguration();
            _epochSize = configuration.EpochSize;
            _isValidator = configuration.NodeIsValidator;
            _startupValidators = configuration.StartupValidatorsWithStakes;

            _stakingStorage = stakingStorage;
            _serviceProvider = serviceProvider;
        }

        public override void CreateGenesisBlockIfNotExist()
        {
            var metaData = GetBlockchainMetadata();
            if (metaData.Nodes < 1)
            {
                var genesisBlock = CreateGenesisBlock();
                AddBlock(genesisBlock);
            }
            else
            {
                var allBlocks = _blockchainRepository.GetBlockchainTree()?.Blocks;
                allBlocks?.ForEach(AddOrUpdateEpoch);
            }
        }

        public override void AddBlock(BlockBase block)
        {
            // Add block to repository
            _blockchainRepository.AddBlock(block);
            AddOrUpdateEpoch(block);

            // Add voting transactions if node is validator
            if (_isValidator)
            {
                AddVotingTransactions();
            }

            //TODO: only for debug
            File.WriteAllText(
                @"D:\Dokumenty\Prace\Projekty\blockchain-simulator\src\BlockchainSimulator\BlockchainSimulator.Hub.WebApi\bin\Debug\netcoreapp2.1\wwwroot\test.json",
                JsonConvert.SerializeObject(_stakingStorage.Epochs));
        }

        public override BlockBase GetLastBlock()
        {
            //TODO override
            var result = base.GetLastBlock();
            return result;
        }

        public override BlockchainTree GetLongestBlockchain()
        {
            //TODO override
            var result = base.GetLongestBlockchain();
            return result;
        }

        private GenesisBlock CreateGenesisBlock()
        {
            var genesisBlock = new GenesisBlock
            {
                Id = "0",
                UniqueId = Guid.Empty.ToString(),
                QueueTime = TimeSpan.Zero,
                Depth = 0,
                Header = new Header
                {
                    Nonce = Guid.Empty.ToString().Replace("-", ""),
                    Target = Guid.Empty.ToString().Replace("-", ""),
                    Version = BlockchainNodeConfiguration.Version,
                    TimeStamp = DateTime.MinValue,
                    MerkleTreeRootHash = null,
                    ParentHash = null
                },
                Body = new Body
                {
                    Transactions = _startupValidators.Select((v, index) => new Transaction
                    {
                        Id = $"{Guid.Empty}-{index}",
                        Sender = v.Key,
                        Recipient = Guid.Empty.ToString(),
                        Amount = v.Value,
                        Fee = 0,
                        RegistrationTime = DateTime.MinValue,
                        TransactionMessage = new TransactionMessage {MessageType = TransactionMessageTypes.Deposit}
                    }).ToHashSet(),
                    MerkleTree = null
                }
            };
            return genesisBlock;
        }

        private void AddOrUpdateEpoch(BlockBase block)
        {
            // Get all messages
            var allMessagedTransactions = block.Body.Transactions.Where(t => t.TransactionMessage != null).ToList();

            if (block.IsGenesis)
            {
                CreateOrUpdateGenesisEpoch(block, allMessagedTransactions);
            }
            else
            {
                CreateOrUpdateEpoch(block, allMessagedTransactions);

                var prepareTransactions = allMessagedTransactions
                    .Where(t => t.TransactionMessage.MessageType == TransactionMessageTypes.Prepare).ToList();
                var commitTransactions = allMessagedTransactions
                    .Where(t => t.TransactionMessage.MessageType == TransactionMessageTypes.Commit).ToList();

                UpdatePrepareEpochs(prepareTransactions);
                UpdateCommitEpochs(commitTransactions);
            }
        }

        private void CreateOrUpdateGenesisEpoch(BlockBase block, List<Transaction> allMessagedTransactions)
        {
            var genesisEpoch = new Epoch(null, 0)
            {
                FinalizedBlockId = block.UniqueId,
                PreparedBlockId = block.UniqueId
            };
            genesisEpoch = _stakingStorage.Epochs.GetOrAdd(0, genesisEpoch);

            var genesisEpochTransactions = genesisEpoch.Transactions;
            allMessagedTransactions.ForEach(t => genesisEpochTransactions.TryAdd(t.Id, t));
            genesisEpoch.CheckpointsWithPrepareStakes.TryAdd(block.UniqueId, genesisEpoch.TotalStake);
        }

        private void CreateOrUpdateEpoch(BlockBase block, List<Transaction> allMessagedTransactions)
        {
            var epochNumber = block.Depth / (_epochSize + 1) + 1;
            if (_stakingStorage.Epochs.TryGetValue(epochNumber - 1, out var previousEpoch))
            {
                var currentEpoch = _stakingStorage.Epochs.GetOrAdd(epochNumber, new Epoch(previousEpoch, epochNumber));
                allMessagedTransactions.ForEach(mt => currentEpoch.Transactions.TryAdd(mt.Id, mt));
            }
        }

        private void UpdatePrepareEpochs(IEnumerable<Transaction> prepareTransactions)
        {
            var transactionsWithMessages = prepareTransactions.Select(t =>
                new {Transaction = t, Message = t.TransactionMessage as PrepareMessage}).ToList();

            transactionsWithMessages.ForEach(tm =>
            {
                if (_stakingStorage.Epochs.TryGetValue(tm.Message.EpochSource, out var sourceEpoch)
                    && _stakingStorage.Epochs.TryGetValue(tm.Message.EpochTarget, out var targetEpoch))
                {
                    if (sourceEpoch.HasPrepared && sourceEpoch.PreparedBlockId == tm.Message.IdSource)
                    {
                        // Update stakes
                        var stakeForValidator = targetEpoch.GetStakeForValidator(tm.Transaction.Sender);
                        targetEpoch.CheckpointsWithPrepareStakes.AddOrUpdate(tm.Message.IdTarget, stakeForValidator,
                            (key, oldValue) => oldValue + stakeForValidator);
                    }
                    
                    // Update prepared
                    var maxStakedCheckpoint = targetEpoch.CheckpointsWithPrepareStakes
                        .OrderByDescending(pair => pair.Value).FirstOrDefault();
                    if (maxStakedCheckpoint.Value >= TwoThird * targetEpoch.TotalStake)
                    {
                        targetEpoch.PreparedBlockId = maxStakedCheckpoint.Key;
                    }
                }
            });
        }

        private void UpdateCommitEpochs(IEnumerable<Transaction> commitTransactions)
        {
            var transactionsWithMessages = commitTransactions.Select(t =>
                new {Transaction = t, Message = t.TransactionMessage as CommitMessage}).ToList();

            transactionsWithMessages.ForEach(tm =>
            {
                if (_stakingStorage.Epochs.TryGetValue(tm.Message.EpochTarget, out var targetEpoch))
                {
                    if (targetEpoch.HasPrepared && targetEpoch.PreparedBlockId == tm.Message.IdTarget)
                    {
                        // Update stakes
                        var stakeForValidator = targetEpoch.GetStakeForValidator(tm.Transaction.Sender);
                        targetEpoch.CheckpointsWithCommitStakes.AddOrUpdate(tm.Message.IdTarget, stakeForValidator,
                            (key, oldValue) => oldValue + stakeForValidator);
                    }
                    
                    // Update prepared
                    var maxStakedCheckpoint = targetEpoch.CheckpointsWithCommitStakes
                        .OrderByDescending(pair => pair.Value).FirstOrDefault();
                    if (maxStakedCheckpoint.Value >= TwoThird * targetEpoch.TotalStake)
                    {
                        targetEpoch.FinalizedBlockId = maxStakedCheckpoint.Key;
                    }
                }
            });
        }

        private void AddVotingTransactions()
        {
            var nonFinalizedCheckpointsWithEpochs = _blockchainRepository.GetBlockchainTree().Blocks
                .Where(b => b.Depth > 0 && b.Depth % _epochSize == 0)
                .Select(b => new {Block = b, Epoch = _stakingStorage.Epochs[b.Depth / (_epochSize + 1) + 1]})
                .Where(pair => !pair.Epoch.HasFinalized).ToList();

            var preparedNonFinalizedCheckpoints = nonFinalizedCheckpointsWithEpochs
                .Where(pair => pair.Epoch.HasPrepared && pair.Epoch.PreviousEpoch.HasFinalized).ToList();

            var nonPreparedCheckpointsWithPreparedParents = nonFinalizedCheckpointsWithEpochs
                .Where(pair => !pair.Epoch.HasPrepared && pair.Epoch.PreviousEpoch.HasPrepared).ToList();

            var transactionService = _serviceProvider.GetService<ITransactionService>();
        }
    }
}