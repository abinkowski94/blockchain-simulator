using BlockchainSimulator.Node.BusinessLogic.Model.Block;
using BlockchainSimulator.Node.BusinessLogic.Model.ValidationResults;

namespace BlockchainSimulator.Node.BusinessLogic.Validators
{
    public interface IBlockchainValidator
    {
        ValidationResult Validate(BlockBase blockchain);
    }
}