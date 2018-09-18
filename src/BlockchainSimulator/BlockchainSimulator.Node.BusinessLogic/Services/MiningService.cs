using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : BaseService, IMiningService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IConsensusService _consensusService;
        private readonly IStatisticService _statisticService;
        private readonly IBlockProvider _blockProvider;
        private readonly IBackgroundTaskQueue _queue;

        public MiningService(IBlockchainRepository blockchainRepository, IConsensusService consensusService,
            IStatisticService statisticService, IBlockProvider blockProvider, IBackgroundTaskQueue queue)
        {
            _blockchainRepository = blockchainRepository;
            _consensusService = consensusService;
            _statisticService = statisticService;
            _blockProvider = blockProvider;
            _queue = queue;
        }

        public void MineBlock(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token)
        {
            var miningTask = new Task<BaseResponse<bool>>(() =>
            {
                _statisticService.RegisterMiningAttempt();

                var transactionSet = transactions.ToHashSet();
                var lastBlock = LocalMapper.Map<BlockBase>(_blockchainRepository.GetLastBlock());
                var newBlock = _blockProvider.CreateBlock(transactionSet, enqueueTime, lastBlock);

                return _consensusService.AcceptBlock(newBlock);
            }, token);

            _queue.QueueBackgroundWorkItem(t => miningTask);
            miningTask.Wait(token);
            
            if (!miningTask.Result.IsSuccess)
            {
                _statisticService.RegisterAbandonedBlock();
            }
        }
    }
}