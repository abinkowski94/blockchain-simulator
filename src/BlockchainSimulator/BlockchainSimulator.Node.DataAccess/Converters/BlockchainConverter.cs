using BlockchainSimulator.Node.DataAccess.Converters.Specific;
using BlockchainSimulator.Node.DataAccess.Model;
using Newtonsoft.Json;

namespace BlockchainSimulator.Node.DataAccess.Converters
{
    public static class BlockchainConverter
    {
        public static Blockchain DeserializeBlockchain(string json)
        {
            if (json == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] { new BlockConverter(), new NodeConverter() }
            };
            return JsonConvert.DeserializeObject<Blockchain>(json, settings);
        }
    }
}