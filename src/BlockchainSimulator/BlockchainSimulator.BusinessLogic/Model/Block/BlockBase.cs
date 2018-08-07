using Newtonsoft.Json;

namespace BlockchainSimulator.BusinessLogic.Model.Block
{
    public abstract class BlockBase
    {
        public string Id { get; set; }

        public Body Body { get; set; }

        public Header Header { get; set; }

        public abstract bool IsGenesis { get; }

        public string BlockJson { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}