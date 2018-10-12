using System.Collections.Concurrent;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public class EncodedBlocksStorage : IEncodedBlocksStorage
    {
        public ConcurrentBag<string> EncodedBlocksIds { get; }

        public EncodedBlocksStorage()
        {
            EncodedBlocksIds = new ConcurrentBag<string>();
        }

        public void Clear()
        {
            EncodedBlocksIds.Clear();
        }
    }
}