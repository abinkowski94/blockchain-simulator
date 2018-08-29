using System;
using System.Net.Http;
using System.Threading;

namespace BlockchainSimulator.Common.Services
{
    /// <summary>
    /// The http service
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Requests the http get action
        /// </summary>
        /// <param name="uri">The endpoint</param>
        /// <param name="timeout">Timeout for request</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        HttpResponseMessage Get(string uri, TimeSpan? timeout = null, CancellationToken? token = null);

        /// <summary>
        /// Requests the http put action
        /// </summary>
        /// <param name="uri">The endpoint</param>
        /// <param name="body">The body of the request</param>
        /// <param name="timeout">Timeout for request</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        HttpResponseMessage Put(string uri, HttpContent body, TimeSpan? timeout = null,
            CancellationToken? token = null);

        /// <summary>
        /// Requests the http post action
        /// </summary>
        /// <param name="uri">The endpoint</param>
        /// <param name="body">The body of the request</param>
        /// <param name="timeout">Timeout for request</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        HttpResponseMessage Post(string uri, HttpContent body, TimeSpan? timeout = null,
            CancellationToken? token = null);
    }
}