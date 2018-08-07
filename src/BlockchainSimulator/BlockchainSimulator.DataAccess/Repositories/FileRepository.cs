using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BlockchainSimulator.DataAccess.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly string _directory;

        public FileRepository(IHostingEnvironment environment)
        {
            _directory = environment.WebRootPath;
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