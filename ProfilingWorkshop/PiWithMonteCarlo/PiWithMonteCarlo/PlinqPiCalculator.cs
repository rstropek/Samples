using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
	/// <summary>
	/// Monte carlo written with PLINQ
	/// </summary>
	public static class PlinqPiCalculator
	{
		public static double Calculate(int iterations)
		{
			var random = new ThreadSafeRandom();
			var inCircle = ParallelEnumerable.Range(0, iterations)
				// doesn't make sense to use more threads than we have processors
				.WithDegreeOfParallelism(Environment.ProcessorCount)
				.Select(_ =>
				{
					double a, b;
					return Math.Sqrt((a = random.NextDouble()) * a + (b = random.NextDouble()) * b) <= 1;
				})
				.Aggregate<bool, int, int>(
					0, // Seed
					(agg, val) => val ? agg + 1 : agg, // Iterations
					(agg, subTotal) => agg + subTotal, // Aggregating subtotals
					result => result); // No projection of result needed

			return ((double)inCircle / iterations) * 4;
		}
	}
}
