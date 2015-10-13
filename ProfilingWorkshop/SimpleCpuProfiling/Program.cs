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
			for(var i=0; i<1000; i++)
			{
				var dt = DateTime.Now;
			}
		}

		static void GetSomeUTCDates()
		{
			for (var i = 0; i < 1000; i++)
			{
				var dt = DateTime.UtcNow;
			}
		}

		static void Sleep()
		{
			for (var i=0; i<1000; i++)
			{
				Thread.Sleep(1);
			}
		}

		static void BurnCpuCycles()
		{
			for (var i = 0; i<1000; i++)
			{
				Thread.SpinWait(500000);
			}
		}
	}
}
