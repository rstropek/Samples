using System;

namespace PiWithMonteCarlo
{
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
				var c = Math.Sqrt(a * a + b * b);
				if (c <= 1)
				{
					inCircle++;
				}
			}

			return ((double)inCircle / iterations) * 4;
		}
    }
}
