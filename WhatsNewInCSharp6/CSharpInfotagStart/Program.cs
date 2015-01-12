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
			themes = new List<Theme>
			{
				new Theme("dark", ConsoleColor.Black, ConsoleColor.White),
				new Theme("light", ConsoleColor.White, ConsoleColor.Black),
				new Theme("winter", ConsoleColor.Gray, ConsoleColor.Gray)
				// everything's gray in Austrian winter.
			};

			themesIndex = new Dictionary<string, Theme>();
			themesIndex["dark"] = themes[0];
			themesIndex["light"] = themes[1];
			themesIndex[themes[2].Name] = themes[2];
		}

		private static async Task InitializeThemesAsync()
		{
			var exceptionMessage = string.Empty;
			try
			{
				InitializeThemes();
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					// For demo purposes, lets say we are just interested in the
					// inner exception. If there is no inner exception, we do not
					// want to handle the exception.
					exceptionMessage = ex.InnerException.Message;
				}
				else
				{
					throw;
				}
			}

			// Now that we are ouside of catch, we can use await.
			await LogAsync(exceptionMessage);
		}

		static void Main(string[] args)
		{
			InitializeThemes();
			DoSomethingWithCSharp6();
		}

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

		private static async Task LogAsync(string message)
		{
			// Simulate async logging
			await Task.Delay(100);
			Console.WriteLine(message);
		}
	}
}
