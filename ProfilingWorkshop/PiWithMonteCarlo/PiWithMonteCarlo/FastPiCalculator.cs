using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
	public static class FastPiCalculator
	{
		public static double Calculate(int iterations)
		{
			var procCount = Environment.ProcessorCount;
			if (iterations % procCount != 0)
			{
				throw new ArgumentException("Must be a multiple of Environment.ProcessorCount", "iterations");
			}

			var iterPerProc = iterations / procCount;

			var inCircleLocal = new int[procCount];
			var tasks = new Task[procCount];
			for (var proc = 0; proc < procCount; proc++)
			{
				var procIndex = proc;
				tasks[proc] = Task.Run(() =>
					{
						var inCircleLocalCounter = 0;
						var random = new Random(procIndex);
						for (var index = 0; index < iterPerProc; index++)
						{
							var a = random.NextDouble();
							var b = random.NextDouble();
							var c = Math.Sqrt(a * a + b * b);
							if (c <= 1)
							{
								inCircleLocalCounter++;
							}
						}

						inCircleLocal[procIndex] = inCircleLocalCounter;
					});
			}

			Task.WaitAll(tasks);

			var inCircle = inCircleLocal.Sum();
			return ((double)inCircle / iterations) * 4;
		}
	}
}
