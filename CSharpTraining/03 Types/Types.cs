using System;

static class Types
{
	static void Main()
	{
		var bool_value = true; // Note implicit typing here
		const char char_value = 'x'; // Note the const here
		double double_value = System.Math.PI;
		var int_value = 11;
		System.Int32 int_value_2 = 12;

		Console.WriteLine(bool_value.GetType().ToString());
		Console.WriteLine(char_value.GetType().ToString());
		Console.WriteLine(double_value.GetType().ToString());
		Console.WriteLine(int_value.GetType().ToString());
		Console.WriteLine(int_value_2.GetType().ToString());

		Console.WriteLine(11.GetType().ToString());
		Console.WriteLine(3.1415.GetType().ToString());
		Console.WriteLine('x'.GetType().ToString());

		var string_value = "This is a message!";
		System.String string_value_2 = "This is a second message!";
		object object_value = string_value; // Note the use of object here

		Console.WriteLine(string_value.GetType().ToString());
		Console.WriteLine(string_value_2.GetType().ToString());
		Console.WriteLine(object_value.GetType().ToString());
	}
}