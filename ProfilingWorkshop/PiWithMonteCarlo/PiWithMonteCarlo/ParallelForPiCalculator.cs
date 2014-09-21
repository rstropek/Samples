using System;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
	public static class ParallelForPiCalculator
	{
		public static double Calculate(int iterations)
		{
			var randomLockObject = new object();
			int inCircle = 0;
			var random = new Random();

			Parallel.For(0, iterations, i =>
				{
					double a, b;
					lock (randomLockObject)
					{
						a = random.NextDouble();
						b = random.NextDouble();
					}

					var c = Math.Sqrt(a * a + b * b);
					if (c <= 1)
					{
						Interlocked.Increment(ref inCircle);
					}
				});

			return ((double)inCircle / iterations) * 4;
		}
	}
}
