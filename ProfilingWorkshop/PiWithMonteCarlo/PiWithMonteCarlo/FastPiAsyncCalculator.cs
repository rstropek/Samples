using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
#if LANG_EXPERIMENTAL
    /// <summary>
    /// Note C# 6 primay constructor and auto-property initializer here
    /// </summary>
	public class PiCalculatorIntermediateResult(long iterations, double resultPi)
    {
		public double ResultPi { get; } = resultPi;
		public long Iterations { get; } = iterations;
	}
#else
	public class PiCalculatorIntermediateResult
	{
        public PiCalculatorIntermediateResult(long iterations, double resultPi)
		{
			this.Iterations = iterations;
			this.ResultPi = resultPi;
		}

		public double ResultPi { get; private set; }
		public long Iterations { get; private set; }
	}
#endif

    /// <summary>
    /// Async version of <see cref="FastPiCalculator"/>.
    /// </summary>
    public class FastPiAsyncCalculator
	{
		public static async Task CalculateAsync(CancellationToken cancellationToken, 
			Func<PiCalculatorIntermediateResult, Task> reportIntermediateResultAsyncCallback,
			Func<Task> stoppedCallback)
		{
			// Number of iterations after which we check if it is time to report results back to caller
			const long timerCheckInterval = 100000;

			// Reserve one thread for UI
			var procCount = Environment.ProcessorCount - 1;
			do
			{
				var watch = new Stopwatch();
				watch.Start();

				// One array slot per processor
				var inCircleArray = new long[procCount];
				var iterationsArray = new long[procCount];
				var tasksArray = new Task[procCount];
				for (var proc = 0; proc < procCount; proc++)
				{
					var procIndex = proc; // Helper for closure

					// Start one task per processor
					tasksArray[proc] = Task.Run(() =>
					{
						long inCircleLocalCounter = 0;
						long iterationsLocalCounter = 0;
						var random = new Random(procIndex);
						for (var index = 0; true; index++)
						{
							iterationsLocalCounter++;
							double a, b;
							if (Math.Sqrt((a = random.NextDouble()) * a + (b = random.NextDouble()) * b) <= 1)
							{
								inCircleLocalCounter++;
							}

							if (index >= timerCheckInterval)
							{
								index = 0;
								lock (watch)
								{
									if (watch.ElapsedMilliseconds >= 1000)
									{
										// Every second we stop calculating and report result back
										break;
									}
								}
							}
						}

						// public local counters in result array
						inCircleArray[procIndex] = inCircleLocalCounter;
						iterationsArray[procIndex] = iterationsLocalCounter;
					});
				}

				// Wait until all processing tasks are done
				await Task.WhenAll(tasksArray).ConfigureAwait(false);

				// Report result back
				var inCircle = inCircleArray.Sum();
				var iterations = iterationsArray.Sum();
				await reportIntermediateResultAsyncCallback(
					new PiCalculatorIntermediateResult(iterations, ((double)inCircle / iterations) * 4));
			}
			while (!cancellationToken.IsCancellationRequested);

			// Report end of calculation back to caller
			await stoppedCallback();
		}
	}
}
