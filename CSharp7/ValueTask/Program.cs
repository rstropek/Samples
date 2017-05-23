using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ValueTask
{
    class Program
    {
        static void Main(string[] args)
        {
            PerfTest().Wait();
        }

        #region Performance measurement settings
        private const int NumberOfIterations = 1000000;
        private const int CacheSize = 1000;
        #endregion

        #region Helper methods for performance measurement
        static async Task Measure<T>(Func<ValueTask<T>> body)
        {
            Console.WriteLine($"Starting perf test");
            var sw = Stopwatch.StartNew();
            await body();
            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }


        static async Task Measure<T>(string testName, Func<int, ValueTask<T>> body)
        {
            Console.WriteLine($"Starting perf test {testName}");
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < NumberOfIterations; i++)
            {
                await body(i % CacheSize);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
        static async Task Measure<T>(string testName, Func<int, Task<T>> body)
        {
            Console.WriteLine($"Starting perf test {testName}");
            var sw = Stopwatch.StartNew();
            for (var i = 0; i < NumberOfIterations; i++)
            {
                await body(i % CacheSize);
            }

            sw.Stop();
            Console.WriteLine(sw.Elapsed);
        }
        #endregion

        #region Cache
        /// <summary>
        /// Find out whether result for specified index is in cache
        /// </summary>
        /// <remarks>
        /// This method simulates a mechanism to find out whether an element is in cache.
        /// For this demo, it always returns true because we make sure that all results
        /// are cached.
        /// </remarks>
        private static bool IsInCache(int index) => true;

        private static int[] cache = new int[CacheSize];
        private static Task<int>[] taskCache = new Task<int>[CacheSize];
        private static ValueTask<int>[] valueTaskCache = new ValueTask<int>[CacheSize];
        #endregion

        /// <summary>
        /// Loads the result for specified index.
        /// </summary>
        static async Task<int> LoadResultAsync(int index)
        {
            // Simulate IO by waiting for one ms
            await Task.Delay(1);

            // Simulate successful loading by calculating a result
            return index * index;
        }

        static async Task PerfTest()
        {
            Console.WriteLine("Filling cache...");
            for (var index = 0; index < CacheSize; index++)
            {
                cache[index] = index * index;
                taskCache[index] = Task.FromResult(index * index);
                valueTaskCache[index] = new ValueTask<int>(index * index);
            }

            //await Measure("Task with await", GetResultTaskWithAwait);
            await Measure("Task without await", GetResultTaskWithoutAwait);
            //await Measure("Task with Task cache", GetResultTaskWithTaskCache);
            //await Measure("ValueTask with await", GetResultValueTaskWithAwait);
            await Measure("ValueTask without await", GetResultValueTaskWithoutAwait);
            //await Measure("ValueTask with ValueTask cache", GetResultValueTaskWithValueTaskCache);
        }

        private static async Task<int> GetResultTaskWithAwait(int index) 
            => IsInCache(index) ? cache[index] : await LoadResultAsync(index);

        private static Task<int> GetResultTaskWithoutAwait(int index) 
            => IsInCache(index) ? Task.FromResult(cache[index]) : LoadResultAsync(index);

        private static Task<int> GetResultTaskWithTaskCache(int index) 
            => IsInCache(index) ? taskCache[index] : LoadResultAsync(index);

        private static async ValueTask<int> GetResultValueTaskWithAwait(int index) 
            => IsInCache(index) ? cache[index] : await LoadResultAsync(index);

        static ValueTask<int> GetResultValueTaskWithoutAwait(int index) 
            => IsInCache(index) ? new ValueTask<int>(cache[index]) : new ValueTask<int>(LoadResultAsync(index));

        static ValueTask<int> GetResultValueTaskWithValueTaskCache(int index) 
            => IsInCache(index) ? valueTaskCache[index] : new ValueTask<int>(LoadResultAsync(index));
    }
}