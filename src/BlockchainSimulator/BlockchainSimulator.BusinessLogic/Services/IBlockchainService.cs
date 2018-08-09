using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.Transaction;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IBlockchainService
    {
        ValidationResult Validate(BlockBase blockchain);

        BlockBase CreateBlock(HashSet<Transaction> transactions, BlockBase block = null);
    }
}