using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace BlockchainSimulator.Common.Extensions
{
    /// <summary>
    /// The linq extensions
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// The foreach extension
        /// </summary>
        /// <param name="source">The source collection</param>
        /// <param name="action">The action to perform</param>
        /// <typeparam name="T">The generic type</typeparam>
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        /// <summary>
        /// The parallel foreach
        /// </summary>
        /// <param name="source">The source collection</param>
        /// <param name="action">The action</param>
        /// <param name="token">The cancellation token</param>
        /// <typeparam name="T">The generic type</typeparam>
        public static void ParallelForEach<T>(this IEnumerable<T> source, Action<T> action,
            CancellationToken? token = null)
        {
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = token ?? CancellationToken.None,
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(source, parallelOptions, (element, state) =>
            {
                if (!parallelOptions.CancellationToken.IsCancellationRequested)
                {
                    action(element);
                }
            });
        }

        /// <summary>
        /// Sums the timespans
        /// </summary>
        /// <param name="source">The collection</param>
        /// <param name="timeSpan">The requested timespan</param>
        /// <typeparam name="T">The generic type</typeparam>
        /// <returns>The sum of timespans</returns>
        public static TimeSpan Sum<T>(this IEnumerable<T> source, Func<T, TimeSpan> timeSpan)
        {
            var result = new TimeSpan();
            source.ForEach(e => result += timeSpan(e));

            return result;
        }
    }
}