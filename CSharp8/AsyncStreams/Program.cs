using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
        // beauty of async and IEnumerable.
        static async IAsyncEnumerable<string> GetCustomerNamesAsync(IEnumerable<int> customerIds)
        {
            foreach(var customerId in customerIds)
            {
                var name = await GetCustomerNameAsync(customerId);
                yield return name;
            }
        }

        static async Task Main()
        {
            // Note await foreach here. It makes iterating over an async enumerable
            // very easy.
            await foreach(var customer in GetCustomerNamesAsync(new[] { 1, 2, 3 }))
            {
                Console.WriteLine(customer);
            }
        }
    }
}
