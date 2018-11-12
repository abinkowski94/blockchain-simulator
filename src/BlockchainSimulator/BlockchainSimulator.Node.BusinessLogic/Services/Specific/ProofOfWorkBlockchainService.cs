using System;
using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfWorkBlockchainService : BaseService, IBlockchainService
    {
        protected readonly IBlockchainRepository _blockchainRepository;

        public ProofOfWorkBlockchainService(IConfigurationService configurationService,
            IBlockchainRepository blockchainRepository) : base(configurationService)
        {
            _blockchainRepository = blockchainRepository;
        }

        public virtual void CreateGenesisBlockIfNotExist()
        {
            var metaData = GetBlockchainMetadata();
            if (metaData.Nodes < 1)
            {
                var genesisBlock =
                    new DataAccess.Model.Block.GenesisBlock
                    {
                        Id = "0",
                        UniqueId = Guid.Empty.ToString(),
                        QueueTime = TimeSpan.Zero,
                        Depth = 0,
                        Header = new DataAccess.Model.Block.Header
                        {
                            Nonce = Guid.Empty.ToString().Replace("-", ""),
                            Target = Guid.Empty.ToString().Replace("-", ""),
                            Version = BlockchainNodeConfiguration.Version,
                            ParentHash = null,
                            TimeStamp = DateTime.MinValue,
                            MerkleTreeRootHash = null
                        },
                        Body = new DataAccess.Model.Block.Body
                            {Transactions = new HashSet<DataAccess.Model.Transaction.Transaction>(), MerkleTree = null}
                    };

                _blockchainRepository.AddBlock(genesisBlock);
            }
        }

        public virtual void AddBlock(DataAccess.Model.Block.BlockBase block)
        {
            _blockchainRepository.AddBlock(block);
        }

        public virtual BlockchainTree GetLongestBlockchain()
        {
            return _blockchainRepository.GetLongestBlockchain();
        }

        public virtual DataAccess.Model.Block.BlockBase GetLastBlock()
        {
            return _blockchainRepository.GetLastBlock();
        }

        public BaseResponse<BlockBase> GetBlockchainTreeLinked()
        {
            var blockchain = _blockchainRepository.GetBlockchainTree();
            if (blockchain?.Blocks == null)
            {
                return new ErrorResponse<BlockBase>("The blockchain tree does not contain blocks!", null);
            }

            var result = LocalMapper.ManualMap(blockchain);
            return new SuccessResponse<BlockBase>("The blockchain from local storage.", result);
        }

        public BlockchainTree GetBlockchainTree()
        {
            return _blockchainRepository.GetBlockchainTree();
        }

        public DataAccess.Model.Block.BlockBase GetBlock(string uniqueId)
        {
            return _blockchainRepository.GetBlock(uniqueId);
        }

        public BlockchainTreeMetadata GetBlockchainMetadata()
        {
            return _blockchainRepository.GetBlockchainMetadata();
        }

        public BlockchainTree GetBlockchainFromBranch(string uniqueId)
        {
            return _blockchainRepository.GetBlockchainFromBranch(uniqueId);
        }

        public bool BlockExists(string blockUniqueId)
        {
            return _blockchainRepository.BlockExists(blockUniqueId);
        }

        public void Clear()
        {
            _blockchainRepository.Clear();
        }
    }
}