using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : IMiningService
    {
        private readonly IBlockProvider _blockProvider;
        private readonly IBlockchainService _blockchainService;
        private readonly IConsensusService _consensusService;

        public MiningService(IBlockchainService blockchainService, IBlockProvider blockProvider,
            IConsensusService consensusService)
        {
            _blockchainService = blockchainService;
            _blockProvider = blockProvider;
            _consensusService = consensusService;
        }

        public Task MineBlocks(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var blockchainResponse = _blockchainService.GetBlockchain();
                var newBlock = _blockProvider.CreateBlock(transactions.ToHashSet(), enqueueTime, blockchainResponse.Result);
                
                _blockchainService.SaveBlockchain(newBlock);
                _consensusService.ReachConsensus();
            }, token);
        }
    }
}