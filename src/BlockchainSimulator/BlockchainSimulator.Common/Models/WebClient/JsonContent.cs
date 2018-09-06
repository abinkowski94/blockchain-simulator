using System.Net.Http;
using System.Text;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Models.WebClient
{
    /// <inheritdoc />
    /// <summary>
    /// The JSON content
    /// </summary>
    public class JsonContent : StringContent
    {
        /// <inheritdoc />
        /// <summary>
        /// The constructor
        /// </summary>
        /// <param name="object">The object to serialize</param>
        public JsonContent(object @object) : base(JsonConvert.SerializeObject(@object), Encoding.UTF8,
            "application/json")
        {
        }
    }
}