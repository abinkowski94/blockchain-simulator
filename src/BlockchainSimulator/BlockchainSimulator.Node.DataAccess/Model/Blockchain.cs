using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Model
{
    public class Blockchain
    {
        [JsonProperty("blocks")] public List<Block.BlockBase> Blocks { get; set; }
    }
}