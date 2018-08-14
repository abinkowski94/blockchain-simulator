using System;
using Newtonsoft.Json;

namespace BlockchainSimulator.WebApi.Models.Responses
{
    public abstract class BaseResponse
    {
        [JsonProperty("id")] 
        public Guid Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("result")]
        public object Result { get; set; }
    }
}