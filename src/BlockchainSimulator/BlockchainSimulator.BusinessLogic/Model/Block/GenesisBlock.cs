namespace BlockchainSimulator.BusinessLogic.Model.Block
{
    public class GenesisBlock : BlockBase
    {
        public override bool IsGenesis => true;
    }
}