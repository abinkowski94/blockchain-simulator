using BlockchainSimulator.Node.DataAccess.Converters;
using BlockchainSimulator.Node.DataAccess.Model;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Repositories
{
    public class BlockchainRepository : IBlockchainRepository
    {
        private readonly string _blockchainFileName;
        private readonly IFileRepository _fileRepository;
        private readonly object _padlock = new object();

        public BlockchainRepository(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _blockchainFileName = "blockchain.json";
        }

        public Blockchain GetBlockchain()
        {
            lock (_padlock)
            {
                var json = _fileRepository.GetFile(_blockchainFileName);
                return BlockchainConverter.DeserializeBlockchain(json);
            }
        }

        public Blockchain SaveBlockchain(Blockchain blockchain)
        {
            lock (_padlock)
            {
                var serializedObject = JsonConvert.SerializeObject(blockchain);
                _fileRepository.SaveFile(serializedObject, _blockchainFileName);

                return blockchain;
            }
        }
    }
}