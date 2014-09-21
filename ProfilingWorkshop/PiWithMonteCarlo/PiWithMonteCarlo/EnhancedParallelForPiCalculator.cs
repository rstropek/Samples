using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
	public static class EnhancedParallelForPiCalculator
	{
		public static double Calculate(int iterations)
		{
			var counterLockObject = new object();
			int inCircle = 0;
			var random = new ThreadSafeRandom();

			Parallel.For(0, iterations, 
				new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, 
				() => 0, (i, _, tLocal) =>
					{
						var a = random.NextDouble();
						var b = random.NextDouble();
						var c = Math.Sqrt(a * a + b * b);
						tLocal += c <= 1 ? 1 : 0;
						return tLocal;
					},
				subTotal => Interlocked.Add(ref inCircle, subTotal));

			return ((double)inCircle / iterations) * 4;
		}
	}
}
