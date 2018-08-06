using Newtonsoft.Json;

namespace BlockchainSimulator.DataAccess.Model.Block
{
    public abstract class BlockBase
    {
        [JsonProperty("id")]
        public string Id { get; }
      
        [JsonProperty("body")]
        public Body Body { get; }
        
        [JsonProperty("header")]
        public Header Header { get; }
        
        [JsonProperty("isGenesis")]
        public abstract bool IsGenesis { get; }

        public BlockBase(Body body, Header header, string id)
        {
            Body = body;
            Header = header;
            Id = id;
        }
    }
}