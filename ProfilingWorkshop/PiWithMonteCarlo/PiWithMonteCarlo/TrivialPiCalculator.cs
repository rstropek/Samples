using System;
#if LANG_EXPERIMENTAL
// Note c# 6 using static here
using System.Math;
#endif

namespace PiWithMonteCarlo
{
	/// <summary>
	/// Trivial, synchronous calculation algorithm
	/// </summary>
    public static class TrivialPiCalculator
    {
		public static double Calculate(int iterations)
		{
			int inCircle = 0;
			var random = new Random();
			for (int i = 0; i < iterations; i++)
			{
				var a = random.NextDouble();
				var b = random.NextDouble();

				// Strictly speaking, we do not need Sqrt here. We could simply drop it and still get the
				// same result. However, this sample should demonstrate some perf topics, too. Therefore
				// it stays there just so the program has to do some math.
#if LANG_EXPERIMENTAL
				var c = Sqrt(a * a + b * b);
#else
				var c = Math.Sqrt(a * a + b * b);
#endif
				if (c <= 1)
				{
					inCircle++;
				}
			}

			return ((double)inCircle / iterations) * 4;
		}
    }
}
