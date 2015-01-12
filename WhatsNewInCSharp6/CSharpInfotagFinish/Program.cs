using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace CSharpInfotag
{
	public class Program
	{
		private static IList<Theme> themes;
		private static IDictionary<string, Theme> themesIndex;

		private static void InitializeThemes()
		{
			// Collection Initializers
			themes = new List<Theme>
			{
				new Theme("dark", ConsoleColor.Black, ConsoleColor.White),
				new Theme("light", ConsoleColor.White, ConsoleColor.Black),
				new Theme("winter", ConsoleColor.Gray, ConsoleColor.Gray)
					// everything's gray in Austrian winter.
			};

			// Index Initializers
			themesIndex = new Dictionary<string, Theme>
			{
				["dark"] = themes[0],
				["light"] = themes[1],
				[themes[2].Name] = themes[2]
					// Note the usage of an expression as the index here
			};
		}

		private static async Task InitializeThemesAsync()
		{
			var exceptionMessage = string.Empty;
			try
			{
				InitializeThemes();
			}
			// Note the exception filter here.
			catch (Exception ex) if (ex.InnerException != null)
			{
				// Note the usage of await inside of catch
				await LogAsync(exceptionMessage);
			}
		}

		static void Main(string[] args)
		{
			InitializeThemes();
			DoSomethingWithCSharp6();
		}

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

		private static async Task LogAsync(string message)
		{
			// Simulate async logging
			await Task.Delay(100);
			Console.WriteLine(message);
		}
	}
}
