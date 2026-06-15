// ============================================================================
//  Runtime Async is transparent to your code: every async pattern you already
//  use keeps working. These all compile to runtime-async methods (flags(2000),
//  AsyncHelpers.Await) rather than state machines.
// ============================================================================
using System.Diagnostics;

internal static class AsyncPatterns
{
    public static async Task RunAllAsync()
    {
        Console.WriteLine(new string('=', 60));
        Console.WriteLine("Async patterns under runtime-async:");
        Console.WriteLine($"  await in a loop      -> {await SumAsync(5)}");        // 15
        Console.WriteLine($"  try/catch/finally    -> {await SafeDivideAsync(10, 0)}");
        Console.WriteLine($"  ValueTask            -> {await FastPathAsync(true)}");
        Console.WriteLine($"  Task.WhenAll         -> {string.Join(",", await Task.WhenAll(One(), Two(), Three()))}");
    }

    static async Task<int> SumAsync(int n)
    {
        int total = 0;
        for (int i = 1; i <= n; i++)
        {
            await Task.Yield();          // real suspension point
            total += i;
        }
        return total;
    }

    static async Task<string> SafeDivideAsync(int a, int b)
    {
        try
        {
            await Task.CompletedTask;
            return (a / b).ToString();
        }
        catch (DivideByZeroException)
        {
            return "caught divide-by-zero";   // exception flow is preserved
        }
        finally
        {
            Debug.Assert(true);               // finally still runs
        }
    }

    static async ValueTask<int> FastPathAsync(bool ready)
    {
        if (ready) return 42;                 // completes synchronously
        await Task.Yield();
        return -1;
    }

    static async Task<int> One()   { await Task.Yield(); return 1; }
    static async Task<int> Two()   { await Task.Yield(); return 2; }
    static async Task<int> Three() { await Task.Yield(); return 3; }
}
