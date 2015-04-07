using System;

class JitTest
{
	public void Method1()
	{
		Console.WriteLine("In Method1!");
	}

	public void Method2()
	{
		Console.WriteLine("In Method2!");
	}
}

static class Start
{
	static void Main()
	{
		var jt = new JitTest();
		Console.WriteLine("Program started 2...");
		Console.ReadLine();

		Console.WriteLine("Calling Method1...");
		jt.Method1();
		Console.ReadLine();

		Console.WriteLine("Calling Method2...");
		jt.Method2();
		Console.ReadLine();

		Console.WriteLine("Calling Method1 again...");
		jt.Method1();
		Console.ReadLine();
	}
}