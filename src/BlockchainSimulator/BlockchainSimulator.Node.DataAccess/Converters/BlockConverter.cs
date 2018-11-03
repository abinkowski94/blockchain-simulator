using System;
using System.Diagnostics.CodeAnalysis;
using BlockchainSimulator.Node.DataAccess.Model.Block;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainSimulator.Node.DataAccess.Converters
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
                var isGenesis = jObject["isGenesis"].Value<bool>();

                return isGenesis
                    ? jObject.ToObject<GenesisBlock>(serializer)
                    : (BlockBase) jObject.ToObject<Block>(serializer);
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