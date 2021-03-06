using System;
using System.Net;
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
            return CallHttpRequest(httpClient => httpClient.GetAsync(uri, token ?? CancellationToken.None), timeout,
                token);
        }

        /// <inheritdoc />
        /// <summary>
        /// Requests the http get action
        /// </summary>
        /// <param name="uri">The endpoint</param>
        /// <param name="timeout">Timeout for request</param>
        /// <param name="retryCount">The retry count</param>
        /// <param name="token">The cancellation token</param>
        /// <returns>The http response message</returns>
        public HttpResponseMessage Get(string uri, TimeSpan timeout, int retryCount, CancellationToken? token = null)
        {
            if (retryCount < 1)
            {
                throw new ArgumentException($"{nameof(retryCount)} must be greater than zero!", nameof(retryCount));
            }

            var exception = new TimeoutException("The retry count for has been exceeded!");
            while (retryCount > 0)
            {
                try
                {
                    return CallHttpRequest(httpClient => httpClient.GetAsync(uri, token ?? CancellationToken.None),
                        timeout, token);
                }
                catch (Exception e)
                {
                    exception = new TimeoutException("The retry count for has been exceeded!", e);
                }

                retryCount--;
            }

            throw exception;
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
            return CallHttpRequest(httpClient => httpClient.PostAsync(uri, body, token ?? CancellationToken.None),
                timeout, token);
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
            return CallHttpRequest(httpClient => httpClient.PutAsync(uri, body, token ?? CancellationToken.None),
                timeout, token);
        }

        private static HttpResponseMessage CallHttpRequest(Func<HttpClient, Task<HttpResponseMessage>> func,
            TimeSpan? timeout = null, CancellationToken? token = null)
        {
            using (var httpClientHandler = new HttpClientHandler())
            {
                // Turns off SSL
                httpClientHandler.ServerCertificateCustomValidationCallback = (msg, cert, ch, err) => true;
                var defaultTimeout = TimeSpan.FromSeconds(30);

                using (var httpClient = new HttpClient(httpClientHandler) { Timeout = timeout ?? defaultTimeout })
                {
                    try
                    {
                        var responseTask = func(httpClient);
                        if (responseTask.IsCanceled)
                        {
                            return new HttpResponseMessage(HttpStatusCode.BadRequest);
                        }

                        responseTask.Wait(token ?? CancellationToken.None);
                        return responseTask.Result;
                    }
                    catch (HttpRequestException)
                    {
                        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    }
                    catch (TaskCanceledException)
                    {
                        return new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                    }
                    catch (Exception)
                    {
                        return new HttpResponseMessage(HttpStatusCode.BadRequest);
                    }
                }
            }
        }
    }
}