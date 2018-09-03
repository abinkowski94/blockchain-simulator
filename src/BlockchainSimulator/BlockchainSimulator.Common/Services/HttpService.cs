using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Services
{
    /// <inheritdoc />
    /// <summary>
    /// The http service
    /// </summary>
    public class HttpService : IHttpService
    {
        /// <inheritdoc />
        /// <summary>
        /// Requests the http get action
        /// </summary>
        /// <param name="uri">The endpoint</param>
        /// <param name="timeout">Timeout for request</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        public HttpResponseMessage Get(string uri, TimeSpan? timeout = null, CancellationToken? token = null)
        {
            return CallHttpRequest(httpClient => httpClient.GetAsync(uri, token ?? CancellationToken.None));
        }

        /// <inheritdoc />
        /// <summary>
        /// Requests the http post action
        /// </summary>
        /// <param name="uri">The endpoint</param>
        /// <param name="body">The body of the request</param>
        /// <param name="timeout">Timeout for request</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        public HttpResponseMessage Post(string uri, HttpContent body, TimeSpan? timeout = null,
            CancellationToken? token = null)
        {
            return CallHttpRequest(httpClient => httpClient.PostAsync(uri, body, token ?? CancellationToken.None));
        }

        /// <inheritdoc />
        /// <summary>
        /// Requests the http put action
        /// </summary>
        /// <param name="uri">The endpoint</param>
        /// <param name="body">The body of the request</param>
        /// <param name="timeout">Timeout for request</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        public HttpResponseMessage Put(string uri, HttpContent body, TimeSpan? timeout = null,
            CancellationToken? token = null)
        {
            return CallHttpRequest(httpClient => httpClient.PutAsync(uri, body, token ?? CancellationToken.None));
        }

        private static HttpResponseMessage CallHttpRequest(Func<HttpClient, Task<HttpResponseMessage>> func,
            TimeSpan? timeout = null, CancellationToken? token = null)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                // Turns off SSL
                httpClientHandler.ServerCertificateCustomValidationCallback = (msg, cert, ch, err) => true;
                using (var httpClient = new HttpClient(httpClientHandler)
                { Timeout = timeout ?? TimeSpan.FromSeconds(10) })
                {
                    var responseTask = func(httpClient);
                    responseTask.Wait(token ?? CancellationToken.None);
                    return responseTask.Result;
                }
            }
        }
    }
}