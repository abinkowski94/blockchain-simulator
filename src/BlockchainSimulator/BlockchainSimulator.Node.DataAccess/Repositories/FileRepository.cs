using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly string _directoryPath;

        public FileRepository(IHostingEnvironment environment, IConfiguration configuration)
        {
            var contentRoot = environment.ContentRootPath ?? Directory.GetCurrentDirectory();
            var type = configuration["Node:Type"] ?? "PoW";
            _directoryPath = $"{contentRoot}\\{type}";

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }
        }

        public StreamReader GetFileReader(string fileName)
        {
            var path = $"{_directoryPath}\\{fileName}";
            return File.Exists(path) ? new StreamReader(path) : StreamReader.Null;
        }

        public StreamWriter GetFileWriter(string fileName)
        {
            return new StreamWriter($"{_directoryPath}\\{fileName}");
        }
    }
}