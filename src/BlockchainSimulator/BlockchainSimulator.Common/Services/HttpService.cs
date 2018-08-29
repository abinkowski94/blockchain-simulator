using System;
using System.Net.Http;
using System.Threading;

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
            using (var httpClientHandler = new HttpClientHandler())
            {
                // Turns off SSL
                httpClientHandler.ServerCertificateCustomValidationCallback = (msg, cert, ch, err) => true;
                using (var httpClient = new HttpClient(httpClientHandler)
                    {Timeout = timeout ?? TimeSpan.FromSeconds(10)})
                {
                    var responseTask = httpClient.GetAsync(uri, token ?? CancellationToken.None);
                    responseTask.Wait(token ?? CancellationToken.None);
                    return responseTask.Result;
                }
            }
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
            using (var httpClientHandler = new HttpClientHandler())
            {
                // Turns off SSL
                httpClientHandler.ServerCertificateCustomValidationCallback = (msg, cert, ch, err) => true;
                using (var httpClient = new HttpClient(httpClientHandler)
                    {Timeout = timeout ?? TimeSpan.FromSeconds(10)})
                {
                    var responseTask = httpClient.PutAsync(uri, body, token ?? CancellationToken.None);
                    responseTask.Wait(token ?? CancellationToken.None);
                    return responseTask.Result;
                }
            }
        }
    }
}