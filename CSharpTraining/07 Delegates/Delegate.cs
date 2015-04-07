delegate void Greet(string User);

class Authentication
{
	internal Greet GreetMethod;

	public Authentication(Greet GreetMethod)
	{
		this.GreetMethod = GreetMethod;
	}

	public void Login()
	{
		System.Console.Write("User: ");
		var user = System.Console.ReadLine();
		GreetMethod(user);
		GreetMethod.Invoke(user);
	}
}

static class Start
{
	static void Main()
	{
		var auth = new Authentication(SayHelloWorld);
		auth.Login();
		System.Console.WriteLine(auth.GreetMethod.GetType());

		auth = new Authentication(delegate(string user) { System.Console.WriteLine("Hello, {0}", user); });
		auth.Login();
		System.Console.WriteLine(auth.GreetMethod.GetType());

		auth = new Authentication(user => { System.Console.WriteLine("Hello, {0}", user); });
		auth.Login();
		System.Console.WriteLine(auth.GreetMethod.GetType());
	}

	static void SayHelloWorld(string user)
	{
		System.Console.WriteLine("Hello, {0}!", user);
	}
}

