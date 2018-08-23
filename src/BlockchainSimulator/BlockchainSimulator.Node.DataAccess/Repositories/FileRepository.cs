using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly string _directoryPath;

        public FileRepository(IHostingEnvironment environment, IConfiguration configuration)
        {
            var contentRoot = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
            var type = configuration["Node:Type"] ?? "PoW";
            var nodeId = configuration["Node:Id"] ?? "1";
            _directoryPath = $"{contentRoot}\\{type}\\{nodeId}";

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
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