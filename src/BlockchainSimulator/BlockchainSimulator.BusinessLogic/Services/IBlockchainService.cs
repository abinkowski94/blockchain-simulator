using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IBlockchainService
    {
        ValidationResult Validate(Block blockchain);
    }
}