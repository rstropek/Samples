using System.Diagnostics;
using Data;
using Microsoft.EntityFrameworkCore;

{
    const int ITERATIONS = 2_000_000_000;
    var inCircle = ParallelEnumerable.Range(0, ITERATIONS)
        // doesn't make sense to use more threads than we have processors
        .WithDegreeOfParallelism(Environment.ProcessorCount)
        .Select(_ =>
        {
            double a, b;
            return (a = Random.Shared.NextDouble()) * a + (b = Random.Shared.NextDouble()) * b <= 1;
        })
        .Aggregate(
            0, // Seed
            (agg, val) => val ? agg + 1 : agg, // Iterations
            (agg, subTotal) => agg + subTotal, // Aggregating subtotals
            result => result); // No projection of result needed
    Console.WriteLine((double)inCircle / ITERATIONS * 4);
}

{
    var dataContextFactory = new ApplicationDataContextFactory();
    using var context = dataContextFactory.CreateDbContext(args);

    await context.SeedOrdersIfEmptyAsync(total: 100_000, batchSize: 5_000);

    // Define 12 months to query (all of 2024)
    var startDate = new DateTime(2024, 1, 1);
    var months = Enumerable.Range(0, 12)
        .Select(i => startDate.AddMonths(i))
        .ToList();

    Console.WriteLine($"Starting fan-out query for {months.Count} months using PLinq...");
    var stopwatch = Stopwatch.StartNew();

    // Use thread-local storage for DbContext - one context per thread instead of per month
    var threadLocalContext = new ThreadLocal<ApplicationDataContext>(
        () => dataContextFactory.CreateDbContext(args));

    Order[] result;
    try
    {
        // Use PLinq to query each month in parallel
        result = months
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .SelectMany(monthStart =>
            {
                var monthEnd = monthStart.AddMonths(1);
                var parallelContext = threadLocalContext.Value!;

                var monthlyOrders = parallelContext.Orders.AsNoTracking()
                    .Where(o => o.OrderDate >= monthStart && o.OrderDate < monthEnd)
                    .ToArray();

                Console.WriteLine($"Month {monthStart:yyyy-MM}: {monthlyOrders.Length} orders (Thread {Environment.CurrentManagedThreadId})");
                return monthlyOrders;
            })
            .ToArray();
    }
    finally
    {
        // Cleanup all thread-local contexts
        threadLocalContext.Dispose();
    }

    stopwatch.Stop();
    Console.WriteLine($"\nTotal orders retrieved: {result.Length}");
    Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine($"Average per month: {result.Length / 12.0:F2} orders");
}