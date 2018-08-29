using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.DataAccess.Model
{
    public class Blockchain
    {
        [JsonProperty("blocks")] public List<Block.BlockBase> Blocks { get; set; }
    }
}