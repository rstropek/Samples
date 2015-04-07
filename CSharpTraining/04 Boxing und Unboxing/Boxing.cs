static class Start
{
	private class MyClass
	{
		public int x;
	}

	private struct MyStruct
	{
		// Note that this is an ANTI-PATTERN! Just for demo purposes.
		public int x;
	}

	static void Main()
	{
		var i_fuenf = 5;
		PrintType(i_fuenf);

		var inst1 = new MyClass();
		PrintType(inst1);

		var inst2 = new MyStruct();
		PrintType(inst2);
	}

	static void PrintType(object o)
	{
		System.Console.WriteLine(o.GetType().ToString());

		// This is an ANTI-PATTERN! For demo purposes only. Use as/null-check instead.
		if (o is MyClass)
		{
			((MyClass)o).x = 10;
			System.Console.WriteLine("You passed in MyClass!");
		}
		else if (o is MyStruct)
		{
			// This would not work! Why?
			// ((MyStruct)o).x = 10;
			System.Console.WriteLine("You passed in MyStruct!");
		}
		else
		{
			System.Console.WriteLine("You passed a boxed type!");
		}
	}
}