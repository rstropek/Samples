using BenchmarkDotNet.Running;

namespace ValueTask
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<ValueTaskBenchmark>();
        }
    }
}