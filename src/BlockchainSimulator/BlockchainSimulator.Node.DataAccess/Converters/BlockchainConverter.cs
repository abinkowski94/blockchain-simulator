using BlockchainSimulator.Node.DataAccess.Converters.Specific;
using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Converters
{
    public static class BlockchainConverter
    {
        public static BlockchainTree DeserializeBlockchain(JsonTextReader reader)
        {
            return new JsonSerializer {Converters = {new BlockConverter(), new NodeConverter()}}
                .Deserialize<BlockchainTree>(reader);
        }
        
        public static BlockBase DeserializeBlock(string json)
        {
            if (json == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<BlockBase>(json, new JsonSerializerSettings
            {
                Converters = {new BlockConverter(), new NodeConverter()}
            });
        }
    }
}