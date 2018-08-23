namespace BlockchainSimulator.Node.BusinessLogic.Configurations
{
    public interface IBlockchainConfiguration
    {
        string Version { get; }
        string Target { get; }
        int BlockSize { get; }
    }
}