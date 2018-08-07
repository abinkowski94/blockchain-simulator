using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Providers
{
    public interface IMerkleTreeProvider
    {
        Node GetMerkleTree(HashSet<Transaction> transactions);
    }
}