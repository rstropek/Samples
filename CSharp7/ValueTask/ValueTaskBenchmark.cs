using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

// For a practical application of ValueTask, see
// https://github.com/dotnet/corefx/blob/4ac4ecf5303565a5f6ce1a85ed0baed98ed2db6e/src/Common/src/CoreLib/System/Collections/Generic/IAsyncEnumerator.cs

namespace ValueTask
{
    [SimpleJob(RunStrategy.ColdStart, targetCount: 1)]
    [MemoryDiagnoser]
    public class ValueTaskBenchmark
    {
        #region Performance measurement settings
        private const int NumberOfIterations = 100000000;
        private const int CacheSize = 10000;
        #endregion

        public ValueTaskBenchmark()
        {
            for (var index = 0; index < CacheSize; index++)
            {
                cache[index] = index * index;
                taskCache[index] = Task.FromResult(index * index);
                valueTaskCache[index] = new ValueTask<int>(index * index);
            }
        }

        #region Helper methods for performance measurement
        async Task Measure<T>(string testName, Func<int, ValueTask<T>> body)
        {
            for (var i = 0; i < NumberOfIterations; i++)
            {
                await body(i % CacheSize);
            }
        }
        async Task Measure<T>(string testName, Func<int, Task<T>> body)
        {
            for (var i = 0; i < NumberOfIterations; i++)
            {
                await body(i % CacheSize);
            }
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
        private bool IsInCache(int index) => true;

        private readonly int[] cache = new int[CacheSize];
        private readonly Task<int>[] taskCache = new Task<int>[CacheSize];
        private readonly ValueTask<int>[] valueTaskCache = new ValueTask<int>[CacheSize];
        #endregion

        /// <summary>
        /// Loads the result for specified index.
        /// </summary>
        async Task<int> LoadResultAsync(int index)
        {
            // Simulate IO by waiting for one ms
            await Task.Delay(1);

            // Simulate successful loading by calculating a result
            return index * index;
        }

        [Benchmark]
        public void TestTaskWithAwait() => Measure("Task with await", GetResultTaskWithAwait).Wait();

        [Benchmark]
        public void TestTaskWithoutAwait() => Measure("Task without await", GetResultTaskWithoutAwait).Wait();

        [Benchmark]
        public void TestTaskWithTaskCache() => Measure("Task with Task cache", GetResultTaskWithTaskCache).Wait();

        [Benchmark]
        public void TestValueTaskWithAwait() => Measure("ValueTask with await", GetResultValueTaskWithAwait).Wait();

        [Benchmark]
        public void TestValueTaskWithoutAwait() => Measure("ValueTask without await", GetResultValueTaskWithoutAwait).Wait();

        [Benchmark]
        public void TestValueTaskWithValueCache() => Measure("ValueTask with ValueTask cache", GetResultValueTaskWithValueTaskCache).Wait();

        private async Task<int> GetResultTaskWithAwait(int index) 
            => IsInCache(index) ? cache[index] : await LoadResultAsync(index);

        private Task<int> GetResultTaskWithoutAwait(int index) 
            => IsInCache(index) ? Task.FromResult(cache[index]) : LoadResultAsync(index);

        private Task<int> GetResultTaskWithTaskCache(int index) 
            => IsInCache(index) ? taskCache[index] : LoadResultAsync(index);

        private async ValueTask<int> GetResultValueTaskWithAwait(int index) 
            => IsInCache(index) ? cache[index] : await LoadResultAsync(index);

        ValueTask<int> GetResultValueTaskWithoutAwait(int index) 
            => IsInCache(index) ? new ValueTask<int>(cache[index]) : new ValueTask<int>(LoadResultAsync(index));

        ValueTask<int> GetResultValueTaskWithValueTaskCache(int index) 
            => IsInCache(index) ? valueTaskCache[index] : new ValueTask<int>(LoadResultAsync(index));
    }
}