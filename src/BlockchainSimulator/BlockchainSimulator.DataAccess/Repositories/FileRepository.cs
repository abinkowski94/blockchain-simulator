using System.IO;

namespace BlockchainSimulator.DataAccess.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly string _directory;

        public FileRepository()
        {
            _directory = "//";
        }

        public string GetFile(string fileName)
        {
            return File.ReadAllText($"{_directory}//{fileName}");
        }

        public void SaveFile(string data, string fileName)
        {
            File.WriteAllText($"{_directory}//{fileName}", data);
        }
    }
}