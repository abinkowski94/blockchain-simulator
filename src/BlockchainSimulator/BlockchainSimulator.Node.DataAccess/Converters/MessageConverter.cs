using BlockchainSimulator.Node.DataAccess.Model.Messages;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics.CodeAnalysis;

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
                if (jObject["messageType"] == null)
                {
                    return new TransactionMessage { MessageType = TransactionMessageTypes.None };
                }

                var transactionTypeInt = jObject["messageType"].Value<int>();
                var transactionType = (TransactionMessageTypes)transactionTypeInt;

                switch (transactionType)
                {
                    case TransactionMessageTypes.Commit:
                        return JsonConvert.DeserializeObject<CommitMessage>(jObject.ToString());

                    case TransactionMessageTypes.Prepare:
                        return JsonConvert.DeserializeObject<PrepareMessage>(jObject.ToString());

                    default:
                        return JsonConvert.DeserializeObject<TransactionMessage>(jObject.ToString());
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