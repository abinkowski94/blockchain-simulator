using BlockchainSimulator.Node.BusinessLogic.Model.Transaction;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.BusinessLogic.Providers
{
    public interface IMerkleTreeProvider
    {
        Model.Transaction.Node GetMerkleTree(HashSet<Transaction> transactions);
    }
}