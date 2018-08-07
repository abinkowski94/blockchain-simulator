namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IEncryptionService
    {
        string GetSha256Hash(string value);
    }
}