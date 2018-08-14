using BlockchainSimulator.DataAccess.Converters;
using BlockchainSimulator.DataAccess.Model;
using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Repositories
{
    public class BlockchainRepository : IBlockchainRepository
    {
        private readonly IFileRepository _fileRepository;
        private readonly string _blockchainFileName;

        public BlockchainRepository(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
            _blockchainFileName = "blockchain.json";
        }

        public Blockchain GetBlockchain()
        {
            var json = _fileRepository.GetFile(_blockchainFileName);
            return BlockchainConverter.DeserializeBlockchain(json);
        }

        public Blockchain SaveBlockchain(Blockchain blockchain)
        {
            var serializedObject = JsonConvert.SerializeObject(blockchain);
            _fileRepository.SaveFile(serializedObject, _blockchainFileName);

            return blockchain;
        }
    }
}