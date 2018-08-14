using BlockchainSimulator.DataAccess.Converters.Specific;
using BlockchainSimulator.DataAccess.Model;
using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Converters
{
    public static class BlockchainConverter
    {
        public static string SerializeBlockchain(Blockchain blockchain)
        {
            return blockchain == null ? null : JsonConvert.SerializeObject(blockchain);
        }

        public static Blockchain DeserializeBlockchain(string json)
        {
            if (json == null)
            {
                return null;
            }

            var settings = new JsonSerializerSettings
            {
                Converters = new JsonConverter[] {new BlockConverter(), new NodeConverter()}
            };
            return JsonConvert.DeserializeObject<Blockchain>(json, settings);
        }
    }
}