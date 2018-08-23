using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BlockchainSimulator.Hub.DataAccess.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly string _directoryPath;

        public FileRepository(IHostingEnvironment environment)
        {
            _directoryPath = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
        }

        public string GetFile(string fileName)
        {
            var path = $"{_directoryPath}\\{fileName}";
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        public void SaveFile(string data, string fileName)
        {
            File.WriteAllText($"{_directoryPath}\\{fileName}", data);
        }
    }
}