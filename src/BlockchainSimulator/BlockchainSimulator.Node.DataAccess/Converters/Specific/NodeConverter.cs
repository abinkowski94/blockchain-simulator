using System;
using System.Diagnostics.CodeAnalysis;
using BlockchainSimulator.Node.DataAccess.Model.Transaction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainSimulator.Node.DataAccess.Converters.Specific
{
    public class NodeConverter : JsonConverter
    {
        [ExcludeFromCodeCoverage]
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

                return jObject.ToObject<Model.Transaction.Node>(serializer);
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