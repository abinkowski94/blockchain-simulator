namespace BlockchainSimulator.Hub.DataAccess.Repositories
{
    public interface IFileRepository
    {
        string GetFile(string fileName);

        void SaveFile(string data, string fileName);
    }
}