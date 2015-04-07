using System;

static class Start
{
	static void Main()
	{
		try
		{
			var input = Console.ReadLine();
			var numericInput = int.Parse(input);
			Console.WriteLine("You entered {0}", numericInput);
		}
		catch (FormatException fc)
		{
			Console.WriteLine("A FormatException occurred ({0})!", fc.Message);
		}
		catch (Exception ex)
		{
			Console.WriteLine("An Exception of type {1} occurred ({0})!", ex.Message, ex.GetType().ToString());
		}
		finally
		{
			Console.WriteLine("Finally wurde ausgeführt!");
		}
	}
}