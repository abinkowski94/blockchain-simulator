using System.Collections.Generic;
using System.Linq;
using BlockchainSimulator.Common.Models.Statistics;

namespace BlockchainSimulator.Hub.BusinessLogic.Model.Statistics
{
    public class BlockchainNode
    {
        public int NodesCount =>
            1 + (ChildNodes != null ? (ChildNodes.Any() ? ChildNodes.Sum(n => n.NodesCount) : 0) : 0);

        public int Height => GetHeight();

        public BlockInfo BlockInfo { get; set; }

        public List<BlockchainNode> ChildNodes { get; set; }

        private int GetHeight()
        {
            if (ChildNodes == null)
            {
                return 1;
            }

            if (ChildNodes != null && !ChildNodes.Any())
            {
                return 1;
            }

            return 1 + ChildNodes.Max(n => n.Height);
        }
    }
}