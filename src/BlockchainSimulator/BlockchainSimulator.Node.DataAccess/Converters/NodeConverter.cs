using System;
using System.Diagnostics.CodeAnalysis;
using BlockchainSimulator.Node.DataAccess.Model.Transaction;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainSimulator.Node.DataAccess.Converters
{
    public class NodeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MerkleNode);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                var transactionId = jObject["transactionId"]?.Value<string>();

                return transactionId != null
                    ? jObject.ToObject<Leaf>(serializer)
                    : (MerkleNode) jObject.ToObject<Model.Transaction.Node>(serializer);
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