static class Start
{
	static void Main()
	{
		var intArray = new int[5];

		for (var i = 0; i < intArray.Length; i++)
		{
			intArray[i] = i;
		}

		ChangeFirstArrayEntry(intArray);
		System.Console.WriteLine(intArray[0]);

		var intArray2 = intArray;
		intArray[0] = 0;
		System.Console.WriteLine(intArray2[0]);
	}

	static void ChangeFirstArrayEntry(System.Array<int> a)
	{
		a.SetValue(99, 0);
	}
}
