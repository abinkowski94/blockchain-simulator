using BlockchainSimulator.Node.DataAccess.Converters.Specific;
using BlockchainSimulator.Node.DataAccess.Model;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Converters
{
    public static class BlockchainConverter
    {
        public static BlockchainTree DeserializeBlockchain(string json)
        {
            if (json == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<BlockchainTree>(json, new JsonSerializerSettings
            {
                Converters = {new BlockConverter(), new NodeConverter()}
            });
        }

        public static BlockchainTree DeserializeBlockchain(JsonTextReader reader)
        {
            return new JsonSerializer {Converters = {new BlockConverter(), new NodeConverter()}}
                .Deserialize<BlockchainTree>(reader);
        }
    }
}