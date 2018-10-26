using System;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services.Specific
{
    public class ProofOfStakeBlockchainService : ProofOfWorkBlockchainService
    {
        public ProofOfStakeBlockchainService(IConfigurationService configurationService,
            IBlockchainRepository blockchainRepository) : base(configurationService, blockchainRepository)
        {
        }

        public override BlockBase GetLastBlock()
        {
            throw new NotImplementedException();
        }

        public override BlockchainTree GetLongestBlockchain()
        {
            throw new NotImplementedException();
        }
    }
}