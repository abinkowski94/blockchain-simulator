using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BlockchainSimulator.Common.Queues;
using BlockchainSimulator.Node.BusinessLogic.Configurations;
using BlockchainSimulator.Node.BusinessLogic.Queues;
using BlockchainSimulator.Node.DataAccess.Repositories;
using Microsoft.Extensions.Hosting;

namespace BlockchainSimulator.Node.BusinessLogic.Services
{
    public class ReMiningHostedService : BackgroundService
    {
        private readonly IQueuedHostedServiceSynchronizationContext _queuedHostedServiceSynchronizationContext;
        private readonly IMiningHostedServiceSynchronizationContext _miningHostedServiceSynchronizationContext;
        private readonly IBlockchainConfiguration _blockchainConfiguration;
        private readonly IBlockchainRepository _blockchainRepository;
        private readonly ITransactionService _transactionService;

        public ReMiningHostedService(
            IMiningHostedServiceSynchronizationContext miningHostedServiceSynchronizationContext,
            IQueuedHostedServiceSynchronizationContext queuedHostedServiceSynchronizationContext,
            IBlockchainConfiguration blockchainConfiguration, IBlockchainRepository blockchainRepository,
            ITransactionService transactionService)
        {
            _queuedHostedServiceSynchronizationContext = queuedHostedServiceSynchronizationContext;
            _miningHostedServiceSynchronizationContext = miningHostedServiceSynchronizationContext;
            _blockchainConfiguration = blockchainConfiguration;
            _blockchainRepository = blockchainRepository;
            _transactionService = transactionService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.WhenAll(_miningHostedServiceSynchronizationContext.WaitAsync(),
                    _queuedHostedServiceSynchronizationContext.WaitAsync());

                var pendingTransactions = _transactionService.GetPendingTransactions().Result;
                if (pendingTransactions.Count < _blockchainConfiguration.BlockSize)
                {
                    var longestBlockchainBlocks = _blockchainRepository.GetLongestBlockchain()?.Blocks;
                    if (longestBlockchainBlocks != null)
                    {
                        var longestBlockchainTransactionsIds = longestBlockchainBlocks
                            .SelectMany(b => b.Body.Transactions).Select(t => t.Id).ToList();
                        var transactionsToReMine = _transactionService.RegisteredTransactions.Values
                            .Where(t => !longestBlockchainTransactionsIds.Contains(t.Id))
                            .Where(t => pendingTransactions.All(pt => pt.Id != t.Id)).ToList();

                        if (transactionsToReMine.Any())
                        {
                            transactionsToReMine.ForEach(t => _transactionService.AddTransaction(t));
                        }
                        else
                        {
                            // TODO: Sync with nodes
                        }
                    }
                }
            }
        }
    }
}