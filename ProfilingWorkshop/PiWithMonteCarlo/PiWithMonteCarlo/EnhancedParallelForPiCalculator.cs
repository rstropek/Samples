using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
	/// <summary>
	/// Enhanced version of <see cref="ParallelForPiCalculator"/>.
	/// </summary>
	public static class EnhancedParallelForPiCalculator
	{
		public static double Calculate(int iterations)
		{
			var counterLockObject = new object();
			int inCircle = 0;
			var random = new ThreadSafeRandom();

			Parallel.For(0, iterations,
				// doesn't make sense to use more threads than we have processors
				new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, 
				() => 0, (i, _, tLocal) =>
					{
						double a, b;
						return tLocal += Math.Sqrt((a = random.NextDouble()) * a + (b = random.NextDouble()) * b) <= 1 ? 1 : 0;
					},
				subTotal => Interlocked.Add(ref inCircle, subTotal));

			return ((double)inCircle / iterations) * 4;
		}
	}
}
