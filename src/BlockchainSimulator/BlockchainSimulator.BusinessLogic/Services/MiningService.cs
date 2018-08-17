using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;

namespace BlockchainSimulator.BusinessLogic.Services
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
                var newBlock = _blockProvider.CreateBlock(transactions.ToHashSet(), blockchainResponse.Result);
                
                _blockchainService.SaveBlockchain(newBlock);
                _consensusService.ReachConsensus();
            }, token);
        }
    }
}