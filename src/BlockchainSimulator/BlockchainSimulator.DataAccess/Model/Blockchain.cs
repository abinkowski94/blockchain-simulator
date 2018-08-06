using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model
{
    public class Blockchain
    {
        [JsonProperty("blocks")]
        public List<Block.BlockBase> Blocks { get; }

        public Blockchain()
        {
            Blocks = new List<Block.BlockBase>();
        } 
    }
}