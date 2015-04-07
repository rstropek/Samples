using System;
using System.Text;

static class Start
{
	static void Main()
	{
		var result = string.Empty;
		var rand = new Random();
		
		// First with string operations

		var watch = new System.Diagnostics.Stopwatch();
		watch.Start();
		for (var i = 0; i < 50000; i++)
		{
			result = result + rand.Next(1000).ToString() + "; ";
		}

		GC.Collect();
		Console.WriteLine("{0}...", result.Substring(0, 40));
	
		watch.Stop();
		Console.WriteLine("It took {0} time", watch.Elapsed);

		result = string.Empty;
		
		// Next, with StringBuilder

		watch = new System.Diagnostics.Stopwatch();
		watch.Start();
		var resultBuilder = new StringBuilder(50000 * 3);
		for (int i = 0; i < 50000; i++)
		{
			resultBuilder.Append(rand.Next(1000).ToString());
			resultBuilder.Append("; ");
		}

		GC.Collect();
		Console.WriteLine("{0}...", resultBuilder.ToString().Substring(0, 40));

		watch.Stop();
		Console.WriteLine("It took {0} time", watch.Elapsed);
	}
}
