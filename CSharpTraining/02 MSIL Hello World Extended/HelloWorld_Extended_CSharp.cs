public class C
{
	public void MyMethod()
	{
		System.Console.WriteLine("I am inside a method!");
	}
}

static class Start
{
	static void Main()
	{
		var myInstance = new C();
		myInstance.MyMethod();
		System.Console.WriteLine("Hello World!");
	}
}
