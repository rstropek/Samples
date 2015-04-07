using System;
using System.Collections.Generic;

static class Start
{
	static void Main()
	{
		var StringList = new Stack<string>();

		Console.ReadKey();

		var r = new Random();
		Console.WriteLine("Adding strings; part 1...");
		for (int i = 0; i < 12500000; i++)
		{
			StringList.Push(String.Format("Adding String 1.{0}!", i));
		}

		Console.WriteLine("Freeing strings...");
		for (int i = 0; i < 10000000; i++)
		{
			StringList.Pop();
		}

		//GC.Collect();

		Console.WriteLine("Adding strings; part 2...");
		for (int i = 0; i < 10000000; i++)
		{
			StringList.Push(String.Format("Adding String 2.{0}!", i));
		}
	}
}