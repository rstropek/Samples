using System;

static class Start
{
	static void Main()
	{
		Console.Write("Bitte geben Sie eine ganze Zahl ein: ");
		string Input = Console.ReadLine();

		try
		{
			Console.WriteLine("Sie haben {0} eingegeben!", Convert.ToInt32(Input));

			// the following line does not work because you cannot type-cast from string to int!
			//Console.WriteLine("Sie haben {0} eingegeben!", (int)Input );
		}
		catch (FormatException)
		{
			Console.WriteLine("Sie haben keine gültige Zahl eingegeben!");
		}
	}
}