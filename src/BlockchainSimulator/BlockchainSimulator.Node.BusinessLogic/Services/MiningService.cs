using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.DataAccess.Repositories;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : BaseService, IMiningService
    {
        private readonly IBlockchainRepository _repository;
        private readonly IBlockProvider _blockProvider;
        private readonly IConsensusService _consensusService;
        private readonly IStatisticService _statisticService;
        private readonly object _padlock = new object();

        public MiningService(IBlockProvider blockProvider, IConsensusService consensusService,
            IStatisticService statisticService, IBlockchainRepository repository)
        {
            _blockProvider = blockProvider;
            _consensusService = consensusService;
            _statisticService = statisticService;
            _repository = repository;
        }

        public void MineBlock(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token)
        {
            lock (_padlock)
            {
                _statisticService.RegisterMiningAttempt();

                var transactionSet = transactions.ToHashSet();
                var lastBlock = _repository.GetLastBlock();
                var newBlock =
                    _blockProvider.CreateBlock(transactionSet, enqueueTime, LocalMapper.Map<BlockBase>(lastBlock));

                _consensusService.AcceptBlock(newBlock);
            }
        }
    }
}