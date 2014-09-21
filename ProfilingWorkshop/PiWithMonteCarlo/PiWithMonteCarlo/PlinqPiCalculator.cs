using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
	public static class PlinqPiCalculator
	{
		public static double Calculate(int iterations)
		{
			var random = new ThreadSafeRandom();
			var inCircle = ParallelEnumerable.Range(0, iterations)
				.Select(_ =>
				{
					var a = random.NextDouble();
					var b = random.NextDouble();
					var c = Math.Sqrt(a * a + b * b);
					return c <= 1;
				})
				.Aggregate<bool, int, int>(
					0,
					(agg, val) => val ? agg + 1 : agg,
					(agg, subTotal) => agg + subTotal,
					result => result);

			return ((double)inCircle / iterations) * 4;
		}
	}
}
