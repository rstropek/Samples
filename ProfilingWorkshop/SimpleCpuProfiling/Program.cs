using System;
using System.Threading;

namespace SimpleCpuProfiling
{
	class Program
	{
		static void Main(string[] args)
		{
			HavingFunWithDates();
			HavingFunWithSleeping();
		}

		static void HavingFunWithDates()
		{
			for (var i = 0; i < 5000; i++)
			{
				GetSomeDates();
				GetSomeUTCDates();
			}
		}

		static void HavingFunWithSleeping()
		{
			Sleep();
			BurnCpuCycles();
		}

		static void GetSomeDates()
		{
			var tickSum = 0L;
			for(var i=0; i<1000; i++)
			{
				var dt = DateTime.Now;
				unchecked { tickSum += dt.Ticks; }
			}
		}

		static void GetSomeUTCDates()
		{
			var tickSum = 0L;
			for (var i = 0; i < 1000; i++)
			{
				var dt = DateTime.UtcNow;
				unchecked { tickSum += dt.Ticks; }
			}
		}

		static void Sleep()
		{
			for (var i=0; i<5000; i++)
			{
				Thread.Sleep(1);
			}
		}

		static void BurnCpuCycles()
		{
			for (var i = 0; i<500; i++)
			{
				for (var j = 0; j < 500000; j++) ;
			}
		}
	}
}
