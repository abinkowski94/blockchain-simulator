using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BlockchainSimulator.Common.Models;
using BlockchainSimulator.Node.BusinessLogic.Model.Staking;
using BlockchainSimulator.Node.BusinessLogic.Storage;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using BlockchainSimulator.Node.DataAccess.Model.Messages;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Transaction = BlockchainSimulator.Node.DataAccess.Model.Transaction.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfStakeBlockchainService : ProofOfWorkBlockchainService
    {
        private const decimal TwoThird = 0.666_666_666_666_666_666_666_666_666M;
        private readonly int _epochSize;
        private readonly string _nodeId;
        private readonly bool _isValidator;
        private readonly string _directoryPath;
        private readonly Dictionary<string, int> _startupValidators;
        private readonly IStakingStorage _stakingStorage;
        private readonly IServiceProvider _serviceProvider;
        private ITransactionService _transactionService;

        public ProofOfStakeBlockchainService(IConfigurationService configurationService,
            IBlockchainRepository blockchainRepository, IStakingStorage stakingStorage,
            IServiceProvider serviceProvider, IHostingEnvironment environment) : base(configurationService,
            blockchainRepository)
        {
            var configuration = configurationService.GetConfiguration();
            _epochSize = configuration.EpochSize;
            _nodeId = configuration.NodeId;
            _isValidator = configuration.NodeIsValidator;
            _startupValidators = configuration.StartupValidatorsWithStakes;

            _stakingStorage = stakingStorage;
            _serviceProvider = serviceProvider;

            var contentRoot = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
            _directoryPath = $"{contentRoot}\\staking-storage";

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        public override void Clear()
        {
            base.Clear();
            _stakingStorage.Clear();
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

            if (block.Depth % _epochSize == 0)
            {
                File.WriteAllText($@"{_directoryPath}\epochs-{_nodeId}.json",
                    JsonConvert.SerializeObject(_stakingStorage.Epochs));
            }
        }

        public override BlockBase GetLastBlock()
        {
            return GetLongestBlockchain()?.Blocks?.LastOrDefault();
        }

        public override BlockchainTree GetLongestBlockchain()
        {
            var lastPreparedBlock = _stakingStorage.Epochs.Values.LastOrDefault(e => e.HasPrepared);
            if (lastPreparedBlock == null)
            {
                return new BlockchainTree {Blocks = new List<BlockBase>()};
            }

            var blockchainTree = _blockchainRepository.GetBlockchainTree();
            var longestBlockchainStart =
                _blockchainRepository.GetBlockchainFromBranch(lastPreparedBlock.PreparedBlockId);
            var block = longestBlockchainStart.Blocks.LastOrDefault();

            if (block != null)
            {
                var blockchainTreeTail = GetTreeTail(block, blockchainTree.Blocks);
                longestBlockchainStart.Blocks.AddRange(blockchainTreeTail);
            }

            return longestBlockchainStart;
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
            genesisEpoch.CheckpointsWithCommitStakes.TryAdd(block.UniqueId, genesisEpoch.TotalStake);
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
            else
            {
                Console.WriteLine($"Could not find epoch parent {epochNumber - 1}\nDepth: {block.Depth}");
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
            _transactionService = _transactionService ?? _serviceProvider.GetService<ITransactionService>();

            var nonFinalizedCheckpointsWithEpochs = _blockchainRepository.GetBlockchainTree().Blocks
                .Where(b => b.Depth > 0 && b.Depth % _epochSize == 0)
                .Select(b => new
                {
                    Block = b,
                    Epoch = _stakingStorage.Epochs.TryGetValue(b.Depth / (_epochSize + 1) + 1, out var epoch)
                        ? epoch
                        : null
                })
                .Where(pair => pair.Epoch != null)
                .Where(pair => !pair.Epoch.HasFinalized).ToList();

            var preparedNonFinalizedCheckpoint = nonFinalizedCheckpointsWithEpochs
                .Where(pair => pair.Epoch.HasPrepared && GetFinalizedEpochAncestor(pair.Epoch) != null)
                .Where(pair => !_stakingStorage.NodesVotes.Contains($"{pair.Epoch.Number}-COMMIT"))
                .OrderByDescending(pair =>
                    pair.Epoch.CheckpointsWithCommitStakes.TryGetValue(pair.Block.UniqueId, out var result)
                        ? result
                        : 0)
                .ThenByDescending(pair =>
                    pair.Epoch.CheckpointsWithPrepareStakes.TryGetValue(pair.Block.UniqueId, out var result)
                        ? result
                        : 0)
                .FirstOrDefault();

            var nonPreparedCheckpointsWithPreparedParent = nonFinalizedCheckpointsWithEpochs
                .Where(pair => !pair.Epoch.HasPrepared && GetPreparedEpochAncestor(pair.Epoch) != null)
                .Where(pair => !_stakingStorage.NodesVotes.Contains($"{pair.Epoch.Number}-PREPARE"))
                .OrderByDescending(pair =>
                    pair.Epoch.CheckpointsWithCommitStakes.TryGetValue(pair.Block.UniqueId, out var result)
                        ? result
                        : 0)
                .ThenByDescending(pair =>
                    pair.Epoch.CheckpointsWithPrepareStakes.TryGetValue(pair.Block.UniqueId, out var result)
                        ? result
                        : 0)
                .FirstOrDefault();

            if (preparedNonFinalizedCheckpoint != null)
            {
                _stakingStorage.NodesVotes.Add($"{preparedNonFinalizedCheckpoint.Epoch.Number}-COMMIT");

                _transactionService.AddTransaction(new Model.Transaction.Transaction
                {
                    Amount = 0,
                    Fee = 0,
                    Recipient = Guid.Empty.ToString(),
                    Sender = _nodeId,
                    TransactionMessage = new Model.Messages.CommitMessage
                    {
                        EpochTarget = preparedNonFinalizedCheckpoint.Epoch.Number,
                        IdTarget = preparedNonFinalizedCheckpoint.Block.UniqueId,
                        MessageType = Model.Messages.TransactionMessageTypes.Commit
                    }
                });
            }
            else if (nonPreparedCheckpointsWithPreparedParent != null)
            {
                _stakingStorage.NodesVotes.Add($"{nonPreparedCheckpointsWithPreparedParent.Epoch.Number}-PREPARE");

                _transactionService.AddTransaction(new Model.Transaction.Transaction
                {
                    Amount = 0,
                    Fee = 0,
                    Recipient = Guid.Empty.ToString(),
                    Sender = _nodeId,
                    TransactionMessage = new Model.Messages.PrepareMessage
                    {
                        IdTarget = nonPreparedCheckpointsWithPreparedParent.Block.UniqueId,
                        EpochTarget = nonPreparedCheckpointsWithPreparedParent.Epoch.Number,
                        IdSource = GetFinalizedEpochAncestor(nonPreparedCheckpointsWithPreparedParent.Epoch).PreparedBlockId,
                        EpochSource = GetFinalizedEpochAncestor(nonPreparedCheckpointsWithPreparedParent.Epoch).Number,
                        MessageType = Model.Messages.TransactionMessageTypes.Prepare
                    }
                });
            }
        }

        private static List<BlockBase> GetTreeTail(BlockBase rootBlock, List<BlockBase> blockchainTreeBlocks)
        {
            var maxDepth = blockchainTreeBlocks.OrderByDescending(b => b.Depth).FirstOrDefault()?.Depth ?? 0;
            var treeItems = blockchainTreeBlocks.Select(b => new TreeItem<BlockBase> {Item = b}).ToList();

            treeItems.ForEach(treeItem =>
            {
                treeItem.Height = maxDepth - treeItem.Item.Depth;
                treeItem.Children = treeItems.Where(childTreeItem => childTreeItem.Item is Block blockWithChildren &&
                                                                     blockWithChildren.ParentUniqueId ==
                                                                     treeItem.Item.UniqueId).ToList();
                treeItem.Children.ForEach(child => child.Parent = treeItem);
            });

            var result = new List<BlockBase>();

            var children = treeItems.FirstOrDefault(treeItem => treeItem.Item.UniqueId == rootBlock.UniqueId)?.Children;
            if (children != null)
            {
                do
                {
                    var child = children.OrderByDescending(c => c.Height).FirstOrDefault();
                    if (child != null)
                    {
                        result.Add(child.Item);
                        children = child.Children;
                    }
                } while (children.Any());
            }

            return result;
        }

        private static Epoch GetFinalizedEpochAncestor(Epoch epoch)
        {
            var ancestor = epoch.PreviousEpoch;
            while (ancestor != null)
            {
                if (ancestor.HasFinalized)
                {
                    return ancestor;
                }
                
                ancestor = ancestor.PreviousEpoch;
            }

            return null;
        }
        
        private static Epoch GetPreparedEpochAncestor(Epoch epoch)
        {
            var ancestor = epoch.PreviousEpoch;
            while (ancestor != null)
            {
                if (ancestor.HasPrepared)
                {
                    return ancestor;
                }
                
                ancestor = ancestor.PreviousEpoch;
            }

            return null;
        }
    }
}