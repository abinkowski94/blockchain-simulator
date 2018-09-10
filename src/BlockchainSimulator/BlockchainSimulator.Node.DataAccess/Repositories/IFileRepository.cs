using System.IO;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IFileRepository
    {
        StreamReader GetFile(string fileName);

        bool SaveFile(string data, string fileName);
    }
}