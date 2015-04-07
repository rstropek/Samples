class Person
{
	private string firstName;
	private string lastName;

	public string FirstName
	{
		get { return firstName; }
		set { firstName = value; }
	}

	public string LastName
	{
		set { lastName = value; }
	}

	public string FirstNameAuto { get; set; }

	public string LastNameAuto { private get; set; }
}

static class Start
{
	static void Main()
	{
		Person x = new Person();
		x.FirstName = "Rainer";
		x.LastName = "Stropek";
	}
}

