using System.Collections.Concurrent;

namespace BlockchainSimulator.Node.BusinessLogic.Storage
{
    public interface IEncodedBlocksStorage
    {
        ConcurrentBag<string> EncodedBlocksIds { get; }
        void Clear();
    }
}