using BlockchainSimulator.Node.BusinessLogic.Model.Responses;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using BlockchainSimulator.Node.BusinessLogic.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class MiningService : BaseService, IMiningService
    {
        private readonly IBlockchainService _blockchainService;
        private readonly IBlockProvider _blockProvider;
        private readonly IConsensusService _consensusService;
        private readonly IStatisticService _statisticService;
        private readonly object _padlock = new object();

        public MiningService(IBlockchainService blockchainService, IBlockProvider blockProvider,
            IConsensusService consensusService, IStatisticService statisticService)
        {
            _blockchainService = blockchainService;
            _blockProvider = blockProvider;
            _consensusService = consensusService;
            _statisticService = statisticService;
        }

        public void MineBlock(IEnumerable<Transaction> transactions, DateTime enqueueTime, CancellationToken token)
        {
            lock (_padlock)
            {
                var transactionSet = transactions.ToHashSet();
                var blockchainResponse = _blockchainService.GetBlockchain();

                _statisticService.RegisterMiningAttempt();
                var newBlock = _blockProvider.CreateBlock(transactionSet, enqueueTime, blockchainResponse.Result);

                var response = _consensusService.AcceptBlockchain(newBlock);
                if (response is ErrorResponse<bool> errorResponse && !errorResponse.Result)
                {
                    _statisticService.RegisterAbandonedBlock();
                    MineBlock(transactionSet, enqueueTime, token);
                }
            }
        }
    }
}