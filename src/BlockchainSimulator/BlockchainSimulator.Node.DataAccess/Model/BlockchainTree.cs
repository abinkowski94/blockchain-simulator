using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.DataAccess.Model
{
    public class BlockchainTree
    {
        [JsonProperty("blocks", Order = 1)]
        public List<Block.BlockBase> Blocks { get; set; }
    }
}