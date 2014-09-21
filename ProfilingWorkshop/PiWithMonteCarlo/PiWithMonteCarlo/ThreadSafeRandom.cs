using System;
using System.Threading;

namespace PiWithMonteCarlo
{
	public class ThreadSafeRandom
	{
		private static Random _seed = new Random();

		[ThreadStatic]
		private static Random _local = null;

		public double NextDouble()
		{
			if (_local == null)
			{
				lock (_seed)
				{
					if (_local == null)
					{
						_local = new Random(_seed.Next(0, Int32.MaxValue));
					}
				}
			}

			return _local.NextDouble();
		}
	}
}
