using System.Collections.Generic;
using BlockchainSimulator.BusinessLogic.Model.Transaction;

namespace BlockchainSimulator.BusinessLogic.Services
{
    public interface IMiningService
    {
        void MineBlocks(List<Transaction> transactions);
    }
}