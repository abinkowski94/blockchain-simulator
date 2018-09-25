using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using BlockchainSimulator.Node.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : BaseService, IMiningService
    {
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly IConsensusService _consensusService;
        private readonly IStatisticService _statisticService;
        private readonly IBlockProvider _blockProvider;

        public MiningService(IBlockchainRepository blockchainRepository, IConsensusService consensusService,
            IStatisticService statisticService, IBlockProvider blockProvider)
        {
            _blockchainRepository = blockchainRepository;
            _consensusService = consensusService;
            _statisticService = statisticService;
            _blockProvider = blockProvider;
        }

        public void MineBlock(HashSet<Transaction> transactions, DateTime enqueueTime, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                _statisticService.RegisterMiningAttempt();

                var cancellationTokenSource = new CancellationTokenSource();
                var lastBlock = LocalMapper.Map<BlockBase>(_blockchainRepository.GetLastBlock());
                var newBlockTask = _blockProvider.CreateBlock(transactions, enqueueTime, lastBlock,
                    cancellationTokenSource.Token);

                while (!newBlockTask.IsCompleted)
                {
                    if (lastBlock?.UniqueId != _blockchainRepository.GetLastBlock()?.UniqueId)
                    {
                        cancellationTokenSource.Cancel();
                    }
                    
                    if (newBlockTask.IsCanceled)
                    {
                        cancellationTokenSource = new CancellationTokenSource();
                        lastBlock = LocalMapper.Map<BlockBase>(_blockchainRepository.GetLastBlock());
                        newBlockTask = _blockProvider.CreateBlock(transactions, enqueueTime, lastBlock,
                            cancellationTokenSource.Token);
                        
                        _statisticService.RegisterAbandonedBlock();
                    }
                }


                var result = _consensusService.AcceptBlock(newBlockTask.Result);
                if (!result.IsSuccess)
                {
                    _statisticService.RegisterAbandonedBlock();
                }
            }
        }
    }
}