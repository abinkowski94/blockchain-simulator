using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Providers;
using BlockchainSimulator.BusinessLogic.Queue;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public class MiningService : IMiningService
    {
        private readonly IBackgroundTaskQueue _queue;
        private readonly IBlockProvider _blockProvider;
        private readonly IBlockchainService _blockchainService;
        private readonly IConsensusService _consensusService;

        public MiningService(IBlockchainService blockchainService, IBlockProvider blockProvider,
            IBackgroundTaskQueue queue, IConsensusService consensusService)
        {
            _blockchainService = blockchainService;
            _blockProvider = blockProvider;
            _queue = queue;
            _consensusService = consensusService;
        }

        public void MineBlocks(List<Transaction> transactions)
        {
            _queue.QueueBackgroundWorkItem(token =>
            {
                return Task.Run(() =>
                {
                    var parentBlock = _blockchainService.Blockchain;
                    var newBlock = _blockProvider.CreateBlock(transactions.ToHashSet(), parentBlock);
                    _blockchainService.SaveBlockchain(newBlock);
                    _consensusService.ReachConsensus(newBlock);
                }, token);
            });
        }
    }
}