namespace BlockchainSimulator.Node.BusinessLogic.Configurations
{
    public interface IBlockchainConfiguration
    {
        int BlockSize { get; }
        string Target { get; }
        string Version { get; }
    }
}