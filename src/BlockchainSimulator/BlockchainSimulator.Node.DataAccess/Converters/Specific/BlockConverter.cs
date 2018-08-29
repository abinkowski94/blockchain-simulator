using BlockchainSimulator.Node.DataAccess.Model.Block;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;

namespace BlockchainSimulator.Node.DataAccess.Converters.Specific
{
    public class BlockConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(BlockBase);
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

        [ExcludeFromCodeCoverage]
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
        }
    }
}