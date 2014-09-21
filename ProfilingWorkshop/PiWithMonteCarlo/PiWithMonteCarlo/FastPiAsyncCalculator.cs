using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PiWithMonteCarlo
{
	public class PiCalculatorIntermediateResult
	{
		public PiCalculatorIntermediateResult(int iterations, double resultPi)
		{
			this.Iterations = iterations;
			this.ResultPi = resultPi;
		}

		public double ResultPi { get; private set; }
		public int Iterations { get; private set; }
	}

	public class FastPiAsyncCalculator
	{
		public static Task CalculateAsync(CancellationToken cancellationToken, ConcurrentQueue<PiCalculatorIntermediateResult> resultQueue)
		{
			const int timerCheckInterval = 100000;
			var procCount = Environment.ProcessorCount;
			return Task.Run(() =>
				{
					do
					{
						var watch = new Stopwatch();
						watch.Start();

						var inCircleArray = new int[procCount];
						var iterationsArray = new int[procCount];
						var tasksArray = new Task[procCount];
						for (var proc = 0; proc < procCount; proc++)
						{
							var procIndex = proc;
							tasksArray[proc] = Task.Run(() =>
							{
								var inCircleLocalCounter = 0;
								var random = new Random(procIndex);
								for (var index = 0; true; index++)
								{
									var a = random.NextDouble();
									var b = random.NextDouble();
									var c = Math.Sqrt(a * a + b * b);
									if (c <= 1)
									{
										inCircleLocalCounter++;
									}

									if (index >= timerCheckInterval)
									{
										index = 0;
										lock (watch)
										{
											if (watch.ElapsedMilliseconds >= 10000)
											{
												break;
											}
										}
									}
								}

								inCircleArray[procIndex] = inCircleLocalCounter;
							});
						}

						Task.WaitAll(tasksArray);

						var inCircle = inCircleArray.Sum();
						var iterations = iterationsArray.Sum();
						resultQueue.Enqueue(new PiCalculatorIntermediateResult(iterations, ((double)inCircle / iterations) * 4));
					}
					while (!cancellationToken.IsCancellationRequested);
				});
		}
	}
}
