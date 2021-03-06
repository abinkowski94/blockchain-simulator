using System;

namespace BlockchainSimulator.Node.BusinessLogic.Model.Statistics
{
    public class BlockInfo
    {
        public string Id { get; set; }

        public string Nonce { get; set; }

        public DateTime TimeStamp { get; set; }

        public string UniqueId { get; set; }

        public string ParentUniqueId { get; set; }
    }
}