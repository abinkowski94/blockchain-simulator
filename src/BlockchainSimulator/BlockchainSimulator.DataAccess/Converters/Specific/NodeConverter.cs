using System;
using BlockchainSimulator.DataAccess.Model.Transaction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainSimulator.DataAccess.Converters.Specific
{
    public class NodeConverter : JsonConverter
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
                if (jObject["transactionId"]?.Value<string>() != null)
                {
                    return jObject.ToObject<Leaf>(serializer);
                }

                return jObject.ToObject<Node>(serializer);
            }
            catch (JsonReaderException)
            {
                return null;
            }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MerkleNode);
        }
    }
}