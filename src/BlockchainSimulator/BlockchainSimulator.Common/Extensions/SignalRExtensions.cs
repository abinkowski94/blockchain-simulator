using Microsoft.AspNetCore.SignalR.Client;
using System;

namespace BlockchainSimulator.Common.Extensions
{
    /// <summary>
    /// The extensions for SignalR client
    /// </summary>
    public static class SignalRExtensions
    {
        /// <summary>
        /// Invokes a hub method on the server using the specified method name.
        /// </summary>
        /// <typeparam name="T">The type of the returned object</typeparam>
        /// <param name="hubConnection">The hub connection</param>
        /// <param name="methodName">Name of the method on server side</param>
        /// <param name="arg1">The argument object</param>
        /// <returns>The result</returns>
        public static T Invoke<T>(this HubConnection hubConnection, string methodName, object arg1)
        {
            try
            {
                var invokeTask = hubConnection.InvokeAsync<T>(methodName, arg1);
                invokeTask.Wait();

                return invokeTask.Result;
            }
            catch (AggregateException ae)
            {
                Console.WriteLine(ae.Message);
                ae.InnerExceptions.ForEach(e => Console.WriteLine(e.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return default(T);
        }

        /// <summary>
        /// Invokes a hub method on the server using the specified method name.
        /// </summary>
        /// <typeparam name="T">The type of the returned object</typeparam>
        /// <param name="hubConnection">The hub connection</param>
        /// <param name="methodName">Name of the method on server side</param>
        /// <returns>The result</returns>
        public static T Invoke<T>(this HubConnection hubConnection, string methodName)
        {
            try
            {
                var invokeTask = hubConnection.InvokeAsync<T>(methodName);
                invokeTask.Wait();

                return invokeTask.Result;
            }
            catch (AggregateException ae)
            {
                Console.WriteLine(ae.Message);
                ae.InnerExceptions.ForEach(e => Console.WriteLine(e.Message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return default(T);
        }
    }
}