using System;
using BlockchainSimulator.DataAccess.Model.Block;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainSimulator.DataAccess.Converters
{
    public class BlockConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                if (jObject["isGenesis"].Value<bool>())
                {
                    return jObject.ToObject<GenesisBlock>(serializer);
                }

                return jObject.ToObject<Block>(serializer);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BlockBase);
        }
    }
}