using System;

namespace BlockchainSimulator.BusinessLogic.Model.Block
{
    public class Header
    {
        public string Version { get; set; }

        public string ParentHash { get; set; }

        public string MerkleTreeRootHash { get; set; }

        public DateTime TimeStamp { get; set; }

        public string Target { get; set; }

        public string Nonce { get; set; }
    }
}