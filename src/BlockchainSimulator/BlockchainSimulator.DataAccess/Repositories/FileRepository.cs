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
            var path = $"{_directory}//{fileName}";
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        public void SaveFile(string data, string fileName)
        {
            File.WriteAllText($"{_directory}//{fileName}", data);
        }
    }
}