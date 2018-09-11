using System.IO;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public interface IFileRepository
    {
        StreamReader GetFileReader(string fileName);

        StreamWriter GetFileWriter(string fileName);
    }
}