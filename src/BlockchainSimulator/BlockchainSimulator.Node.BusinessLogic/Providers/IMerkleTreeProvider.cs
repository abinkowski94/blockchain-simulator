using System.Collections.Generic;
using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public interface IMerkleTreeProvider
    {
        Model.Transaction.Node GetMerkleTree(HashSet<Transaction> transactions);
    }
}