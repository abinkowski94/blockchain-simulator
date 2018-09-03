using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : IMiningService
    {
        private readonly IBlockchainService _blockchainService;
        private readonly IBlockProvider _blockProvider;
        private readonly IConsensusService _consensusService;
        private readonly IMiningQueue _queue;
        private readonly IStatisticService _statisticService;

        public MiningService(IBlockchainService blockchainService, IBlockProvider blockProvider,
            IConsensusService consensusService, IMiningQueue queue, IStatisticService statisticService)
        {
            _blockchainService = blockchainService;
            _blockProvider = blockProvider;
            _consensusService = consensusService;
            _queue = queue;
            _statisticService = statisticService;
        }

        public void MineBlocks(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token)
        {
            _statisticService.RegisterMiningAttempt();

            var transactionSet = transactions.ToHashSet();
            var blockchainResponse = _blockchainService.GetBlockchain();
            var newBlock = _blockProvider.CreateBlock(transactionSet, enqueueTime, blockchainResponse.Result);

            var response = _consensusService.AcceptBlockchain(newBlock);
            if (response is ErrorResponse<bool> errorResponse && !errorResponse.Result)
            {
                _statisticService.RegisterAbandonedBlock();
                _queue.QueueMiningTask(t => new Task(() => MineBlocks(transactionSet, enqueueTime, token)));
            }
        }
    }
}