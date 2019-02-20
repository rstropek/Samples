using System;
using System.Threading.Tasks;

namespace AsyncMain
{
    class Program
    {
        static async Task<string> GetNameFromDatabaseAsync()
        {
            // Simulate DB access
            await Task.Delay(250);
            return "Dummy";
        }

        // Note that Main is an async function returning task.
        // Tip: Use dnSpy to see in IL code that compiler will generate its own
        //      <Main> method calling your own async Main and awaiting its result.
        static async Task Main()
        {
            await DoWhatYouHaveToDoAsync();
        }

        // Note the use of a default literal expression.
        // This is a new feature of C# 7.1. Read more at
        // http://bit.ly/cs-default-literal
        static async Task DoWhatYouHaveToDoAsync(Func<Task<string>> nameReader = default)
        {
            nameReader = nameReader ?? GetNameFromDatabaseAsync;
            Console.WriteLine(await nameReader());
        }
    }
}
