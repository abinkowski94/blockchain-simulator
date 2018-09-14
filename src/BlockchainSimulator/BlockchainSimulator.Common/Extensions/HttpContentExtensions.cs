using System.Net.Http;
using Newtonsoft.Json;

namespace BlockchainSimulator.Common.Extensions
{
    /// <summary>
    /// The http content extension
    /// </summary>
    public static class HttpContentExtensions
    {
        /// <summary>
        /// Reads http content as string
        /// </summary>
        /// <param name="content">The content</param>
        /// /// <typeparam name="T">The parameter of the response</typeparam>
        /// <returns>The sting</returns>>
        public static T ReadAs<T>(this HttpContent content) where T : class
        {
            var readTask = content.ReadAsStringAsync();
            readTask.Wait();
            var result = readTask.Result;

            return result == null ? null : JsonConvert.DeserializeObject<T>(result);
        }
    }
}