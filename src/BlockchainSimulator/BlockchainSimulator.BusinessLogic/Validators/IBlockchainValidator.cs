using BlockchainSimulator.BusinessLogic.Model.Block;
using BlockchainSimulator.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.BusinessLogic.Validators
{
    public interface IBlockchainValidator
    {
        ValidationResult Validate(BlockBase blockchain);
    }
}