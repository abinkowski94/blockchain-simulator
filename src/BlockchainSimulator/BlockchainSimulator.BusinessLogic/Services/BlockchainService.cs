using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.MappingProfiles;
using BlockchainSimulator.DataAccess.Model;
using BlockchainSimulator.DataAccess.Repositories;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class BlockchainService : BaseService, IBlockchainService
    {
        private readonly object _padlock = new object();
        private readonly IBlockchainRepository _blockchainRepository;

        public BlockchainService(IBlockchainRepository blockchainRepository)
        {
            _blockchainRepository = blockchainRepository;
        }

        public BlockBase GetBlockchain()
        {
            lock (_padlock)
            {
                var blockchain = _blockchainRepository.GetBlockchain();
                var result = LocalMapper.ManualMap(blockchain);

                return result;
            }
        }

        public void SaveBlockchain(BlockBase blockBase, List<BlockBase> blocks = null)
        {
            if (blockBase == null)
            {
                return;
            }

            if (blocks == null)
            {
                blocks = new List<BlockBase>();
            }

            switch (blockBase)
            {
                case Block block:
                    blocks.Insert(0, block);
                    SaveBlockchain(block.Parent, blocks);
                    break;
                case GenesisBlock genesisBlock:
                    blocks.Insert(0, genesisBlock);
                    break;
            }

            var mappedBlocks = LocalMapper.Map<List<DataAccess.Model.Block.BlockBase>>(blocks);
            var blockchain = new Blockchain {Blocks = mappedBlocks};

            lock (_padlock)
            {
                _blockchainRepository.SaveBlockchain(blockchain);
            }
        }
    }
}