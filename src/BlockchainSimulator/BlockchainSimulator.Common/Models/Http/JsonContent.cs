using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace BlockchainSimulator.Common.Models.Http
{
    /// <summary>
    /// The JSON content
    /// </summary>
    public class JsonContent : StringContent
    {
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="object">The object to serialize</param>
        public JsonContent(object @object) : base(JsonConvert.SerializeObject(@object), Encoding.UTF8, "application/json")
        {
        }
    }
}