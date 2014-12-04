using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpInfotag
{
    public class Program
    {
        private static IList<Theme> themes;

        static void Main(string[] args)
        {
            InitializeThemes();
            DoSomethingWithCSharp6();
        }

        private static void DoSomethingWithCSharp6()
        {
            // Read theme from application settings and look it up in theme directory
            // Note the use of the null conditional and null coalesce operators
            var themeConfig = ConfigurationManager.AppSettings["Theme"]?.ToLower() ?? "light";
            var theme = themes.FirstOrDefault(p => p.Name == themeConfig);

            Console.ForegroundColor = theme.ForegroundColor;
            Console.BackgroundColor = theme.BackgroundColor;

            // Option: Use multiple null conditional operators
            var setting = ConfigurationManager.AppSettings["Theme"];
            ConsoleColor? backgroundColor = themes?.FirstOrDefault(p => p.Name == setting?.ToLower())?.BackgroundColor;

            // The following statement handles situation in which themes is null
            var nameOfFirstTheme = themes?[0].Name;
            var nameOfLastTheme = themes[themes.Count - 1]?.Name;
        }

        private static Task LogAsync(string message)
        {
            // Simulate async logging
            return Task.Delay(100);
        }

        private static async void InitializeThemes()
        {
            try
            {
                themes = new List<Theme>()
            {
                new Theme("dark", ConsoleColor.Black, ConsoleColor.White),
                new Theme("light", ConsoleColor.White, ConsoleColor.Black),
                new Theme("winter", ConsoleColor.Gray, ConsoleColor.Gray)
                    // everything's gray in Austrian winter.
                };
            }
            catch (Exception ex) if (ex.InnerException as ArgumentException != null)
            {
                await LogAsync(ex.Message);
            }

            // BTW - you can now initialize dictionaries
            var themesDictionary = new Dictionary<string, ConsoleColor>()
            {
                ["dark"] = ConsoleColor.Black,
                ["light"] = ConsoleColor.White
            };
        }
    }
}
