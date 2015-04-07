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
		var inst1 = new MyClass();
		inst1.x = 10;
		ChangeMyClass(inst1);
		System.Console.WriteLine(inst1.x);

		var inst2 = new MyStruct();
		inst2.x = 10;
		ChangeMyStruct(ref inst2);
		System.Console.WriteLine(inst2.x);

		var inst3 = inst1;
		inst1.x = 5;
		System.Console.WriteLine(inst3.x);

		var inst4 = inst2;
		inst2.x = 5;
		System.Console.WriteLine(inst4.x);

		var inst5 = new MyClass();
		inst5.x = 5;
		System.Console.WriteLine(System.String.Format("inst3 and inst1 are {0}equal!", (inst3 == inst1) ? "" : "not "));
		System.Console.WriteLine(System.String.Format("inst5 and inst1 are {0}equal!", (inst5 == inst1) ? "" : "not "));

		var s1 = "Hello Trainer!";
		System.String s2 = s1;
		s1 = "Hello Trainees!";
		System.Console.WriteLine(s1);
		System.Console.WriteLine(s2);
		System.Console.WriteLine(System.String.Format("s1 and s2 are {0}equal!", (s1==s2) ? "" : "not "));

		var s3 = System.Console.ReadLine();
		System.Console.WriteLine(System.String.Format("s1 and s3 are {0}equal!", (s1==s3) ? "" : "not "));
	}

	static void ChangeMyClass(MyClass inst)
	{
		inst.x = -1;
	}

	static void ChangeMyStruct(ref MyStruct inst)
	{
		inst.x = -1;
	}
}