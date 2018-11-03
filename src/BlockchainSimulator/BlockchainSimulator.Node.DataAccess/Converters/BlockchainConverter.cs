using BlockchainSimulator.Node.DataAccess.Model;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BlockchainSimulator.Node.DataAccess.Converters
{
    public static class BlockchainConverter
    {
        private static readonly JsonSerializer JsonSerializer;
        private static readonly JsonSerializerSettings JsonSerializerSettings;

        static BlockchainConverter()
        {
            JsonSerializerSettings = new JsonSerializerSettings
                {Converters = {new BlockConverter(), new NodeConverter(), new MessageConverter()}};
            JsonSerializer = new JsonSerializer
                {Converters = {new BlockConverter(), new NodeConverter(), new MessageConverter()}};
        }

        public static BlockchainTree DeserializeBlockchain(JsonTextReader reader)
        {
            return JsonSerializer.Deserialize<BlockchainTree>(reader);
        }

        public static BlockBase DeserializeBlock(string json)
        {
            return json == null ? null : JsonConvert.DeserializeObject<BlockBase>(json, JsonSerializerSettings);
        }

        public static IEnumerable<BlockBase> DeserializeBlocks(string json)
        {
            return json == null ? null : JsonConvert.DeserializeObject<List<BlockBase>>(json, JsonSerializerSettings);
        }
    }
}