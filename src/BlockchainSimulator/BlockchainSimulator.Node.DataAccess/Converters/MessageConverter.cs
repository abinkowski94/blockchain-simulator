using System;
using System.Diagnostics.CodeAnalysis;
using BlockchainSimulator.Node.DataAccess.Model.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BlockchainSimulator.Node.DataAccess.Converters
{
    public class MessageConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TransactionMessage);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            try
            {
                var jObject = JObject.Load(reader);
                var transactionType = jObject["messageType"].Value<TransactionMessageTypes>();

                switch (transactionType)
                {
                    case TransactionMessageTypes.Commit:
                        return jObject.ToObject<CommitMessage>(serializer);
                    case TransactionMessageTypes.Prepare:
                        return jObject.ToObject<PrepareMessage>(serializer);
                    default:
                        return jObject.ToObject<TransactionMessage>(serializer);
                }
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