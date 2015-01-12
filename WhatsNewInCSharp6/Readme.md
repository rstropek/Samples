# What's New in C# 6?

This lab demonstrates new feature of C# 6. I have used it
in multiple magazine articles and workshops. Additionally,
I have written a [blog article](http://www.software-architects.com/devblog/2014/12/04/NET-Infoday-Whats-New-in-C-6)
on the topic.

Kudos for this sample to [Roman Schacherl from softaware](http://www.softaware.at/About/Unser-Team)
(MVP for Windows Platform Development). We developed this sample together and
did conference sessions on the topic together, too.

## Null-Conditional Operator

Old code ([CSharpInfotagStart/Program.cs](CSharpInfotagStart/Program.cs)):

```
private static void DoSomethingWithCSharp6()
{
	// Read theme from application settings and look it up in theme directory.
	var themeConfig = ConfigurationManager.AppSettings["Theme"];
	if (themeConfig == null)
	{
		// If setting is missing, use light theme
		themeConfig = "light";
	}
	else
	{
		// Ignore casing so confert theme setting to lowercase
		themeConfig = themeConfig.ToLower();
	}

	const ConsoleColor defaultForegroundColor = ConsoleColor.Black;
	const ConsoleColor defaultBackgroundColor = ConsoleColor.White;

	Theme theme;
	if (themes == null || (theme = themes.FirstOrDefault(
		p => p.Name != null && p.Name.ToLower() == themeConfig)) == null)
	{
		// If themes have not been set up or requested theme could not be found, 
		// use default values
		Console.ForegroundColor = defaultForegroundColor;
		Console.BackgroundColor = defaultBackgroundColor;
	}
	else
	{
		Console.ForegroundColor = theme.ForegroundColor;
		Console.BackgroundColor = theme.BackgroundColor;
	}
}
```

New code ([CSharpInfotagFinish/Program.cs](CSharpInfotagFinish/Program.cs)):

```
private static void DoSomethingWithCSharp6()
{
	// Read theme from application settings and look it up in theme directory.
	// Note the use of the null conditional and null coalesce operators.
	var themeConfig = (ConfigurationManager.AppSettings["Theme"]?.ToLower()) ?? "light";

	const ConsoleColor defaultForegroundColor = ConsoleColor.Black;
	const ConsoleColor defaultBackgroundColor = ConsoleColor.White;

	// Note the use of multiple null conditional operators.
	var theme = themes?.FirstOrDefault(p => p.Name?.ToLower() == themeConfig);

	// If themes have not been set up or requested theme could not be found, 
	// use default values.
	// Note that the null conditional operator in "theme?.ForegroundColor" changes
	// the expression's type from "ConsoleColor" to "Nullable<ConsoleColor>". So
	// the following line would result in a compiler error:
	// ConsoleColor foreground = theme?.ForegroundColor;
	Console.ForegroundColor = theme?.ForegroundColor ?? defaultForegroundColor;
	Console.BackgroundColor = theme?.BackgroundColor ?? defaultBackgroundColor;
}
```
