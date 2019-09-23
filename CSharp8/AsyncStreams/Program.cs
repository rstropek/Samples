using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

// Note that this feature will *NOT* work in .NET Framework 4.x.
// It needs .NET Standard 2.1 and .NET Framework 4.x. will not implement
// this new .NET Standard.

// Read the orginal proposal for this feature:
// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-8.0/async-streams.md

namespace AsyncStreams
{
    public static class Program
    {
        static async Task<string> GetCustomerNameAsync(int customerId)
        {
            // Simulate long-running e.g. DB operation
            await Task.Delay(10);
            return $"Customer{customerId}";
        }

        // Note new return type IAsyncEnumerable that lets us combine the
        // beauty of async and IEnumerable. Check out the implementation of
        // IAsyncEnumerable in
        // https://github.com/dotnet/corefx/blob/release/3.0/src/Common/src/CoreLib/System/Collections/Generic/IAsyncEnumerable.cs

        // Discuss the new interface IAsyncDisposable (see AsyncDisposable.cs)
        // https://github.com/dotnet/corefx/blob/release/3.0/src/Common/src/CoreLib/System/IAsyncDisposable.cs
        static async IAsyncEnumerable<string> GetCustomerNamesAsync(IEnumerable<int> customerIds)
        {
            foreach(var customerId in customerIds)
            {
                var name = await GetCustomerNameAsync(customerId).ConfigureAwait(false);

                // Note the use of yield here. Supported also for async enumerators.
                yield return name;
            }
        }

        static async Task Main()
        {
            // Note await foreach here. It makes iterating over an async enumerable
            // very easy. Note that you can use ConfigureAwait with await foreach.
            // Do we need to recap what ConfigureAwait is (Thread.CurrentThread.ExecutionContext)?
            await foreach (var customer in GetCustomerNamesAsync(new[] { 1, 2, 3 }).ConfigureAwait(false))
            {
                Console.WriteLine(customer);
            }

            // Note that Linq is currently not supported with IAsyncEnumerable.
            // So the following line does not work:
            // GetCustomerNamesAsync(new[] { 1, 2, 3 }).Where(n => !string.IsNullOrEmpty(n));

            // Note await using here. This is how you consume an IAsyncDisposable.
            const string demoFile = @"c:\temp\demo.txt";
            await using (var x = new MyWriterWrapperNew(demoFile))
            {
                await x.WriteLineAsync("asdf").ConfigureAwait(false);
            }

            Console.WriteLine(await File.ReadAllTextAsync(demoFile));
        }
    }
}
