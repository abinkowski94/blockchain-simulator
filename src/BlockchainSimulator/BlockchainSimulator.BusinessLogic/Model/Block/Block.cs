namespace BlockchainSimulator.BusinessLogic.Model.Block
{
    public class Block : BlockBase
    {
        public string ParentId => Parent.Id;

        public Block Parent { get; set; }

        public override bool IsGenesis => false;
    }
}