using System;
using System.Diagnostics;

namespace PiWithMonteCarlo.TestDriver
{
	class Program
	{
		static void Main(string[] args)
		{
			var iterations = 20000000 * Environment.ProcessorCount;

			ExecuteAndPrint("Trivial PI Calculator", TrivialPiCalculator.Calculate, iterations);
			ExecuteAndPrint("(Stupid) Parallel.For PI Calculator", ParallelForPiCalculator.Calculate, iterations);
			ExecuteAndPrint("Parallel.For PI Calculator", EnhancedParallelForPiCalculator.Calculate, iterations);
			ExecuteAndPrint("PLinq PI Calculator", PlinqPiCalculator.Calculate, iterations);
			ExecuteAndPrint("Fast PI Calculator", FastPiCalculator.Calculate, iterations);
		}

		private static void ExecuteAndPrint(string label, Func<int, double> calculation, int iterations)
		{
			Console.WriteLine(label);
			PrintResult(Measure(() => calculation(iterations)), iterations);
		}

		private static void PrintResult(Tuple<double, TimeSpan> r, int iterations)
		{
			Console.WriteLine(
				"{0} ({1:#,##0.0000} sec for {2:#,##0} iterations = {3:#,##0.00} iter/sec)", 
				r.Item1, 
				r.Item2.TotalSeconds, 
				iterations, 
				iterations / r.Item2.TotalSeconds);
		}

		private static Tuple<T, TimeSpan> Measure<T>(Func<T> body)
		{
			var watch = new Stopwatch();
			watch.Start();
			var result = body();
			watch.Stop();
			return new Tuple<T, TimeSpan>(result, watch.Elapsed);
		}
	}
}
