using Demo;

static class Start
{
	static void Main()
	{
		var p = new Person();
		p.FirstName = "Rainer";
		p.LastName = "Stropek";
		System.Console.WriteLine(p);
	}
}