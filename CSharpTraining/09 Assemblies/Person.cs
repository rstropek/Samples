using System.Reflection;

[assembly:AssemblyCompanyAttribute("MyCompany Inc.")]
[assembly:AssemblyVersionAttribute("2.2.3.4")]

namespace Demo
{
	public class Person
	{
		public string FirstName;
		public string LastName;

		public override string ToString()
		{
			return FirstName + " " + LastName;
		}
	}
}