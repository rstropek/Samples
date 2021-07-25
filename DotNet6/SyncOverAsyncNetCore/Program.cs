using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace SyncOverAsyncNetCore
{
    class Program
    {
        static void Main()
        {
            Task.Run(() =>
            {
                var sw = Stopwatch.StartNew();
                var tasks = new List<Task>();
                for (int i = 0; i < 50; i++)
                {
                    // Modern async programming:
                    tasks.Add(DoSomethingSlowAsync());

                    // Sync-over-async
                    // Compare the results of this version between .NET Core 3.1 and .NET 6.
                    // Read more at https://github.com/dotnet/runtime/issues/52558.
                    // tasks.Add(Task.Run(DoSomethingSlow));
                }
                Task.WaitAll(tasks.ToArray());
                Console.WriteLine($"It took {sw.Elapsed} in total");
            });

            Console.WriteLine("Press any key to stop.");
            Console.ReadKey();
        }

        static async Task<int> DoSomethingSlowAsync()
        {
            // This is how async code should look like. It uses async/await and Tasks
            // from bottom to top.

            Console.WriteLine($"Before slow work (Thread {Thread.CurrentThread.ManagedThreadId,2})");
            var sw = Stopwatch.StartNew();
            var result = await Task.Run(async () =>
            {
                // Simulate work that takes 2 seconds (e.g. DB or net access)
                await Task.Delay(TimeSpan.FromMilliseconds(new Random().Next(1900, 2100)));
                return 42;
            });
            Console.WriteLine($"After slow work (Thread {Thread.CurrentThread.ManagedThreadId,2}), took {sw.Elapsed}");
            return result;
        }

        static int DoSomethingSlow()
        {
            // This pattern is called "sync-over-async". It uses async/await and Tasks 
            // in the background but offers a sync API to the caller. You can frequently
            // find this pattern in legacy apps where you use modern code behind the scenes,
            // but you cannot update callers (e.g. legacy UI).
            //
            // For more information about sync-over-async, read
            // https://devblogs.microsoft.com/pfxteam/should-i-expose-synchronous-wrappers-for-asynchronous-methods/

            Console.WriteLine($"Before slow work (Thread {Thread.CurrentThread.ManagedThreadId,2})");
            var sw = Stopwatch.StartNew();
            var result = Task.Run(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                return 42;
            }).Result; // Note the blocking access to the Result property here
            Console.WriteLine($"After slow work (Thread {Thread.CurrentThread.ManagedThreadId,2}), took {sw.Elapsed}");
            return result;
        }
    }
}
