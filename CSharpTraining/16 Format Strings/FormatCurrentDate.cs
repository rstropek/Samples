using System;
using System.Globalization;

static class Start
{
	static void Main()
	{
		var cultures = new string[] { "en-US", "de-AT", "de-DE" };

		foreach (string culture in cultures)
		{
			var ci = new CultureInfo(culture);
			Console.WriteLine("For culture {0} current date and time is {1}", culture, DateTime.Now.ToString(ci.DateTimeFormat));
		}

	}
}