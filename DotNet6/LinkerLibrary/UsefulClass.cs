#nullable enable
#pragma warning disable CA1822 // Mark members as static

using System.Reflection;
using System.Linq;
using System.Diagnostics.CodeAnalysis;

namespace LinkerLibrary
{
    public class UsefulClass
    {
        public int GetAnswer() => 42;

        [RequiresUnreferencedCode("Dynamically references System.Text.Json")]
        public string Serialize()
        {
            var a = Assembly.Load("System.Text.Json");
            var t = a!.GetType("System.Text.Json.JsonSerializer");
            var mi = t!.GetMethods().First(m => m.Name == "Serialize" && !m.IsGenericMethod);
            var resultDynamic = mi!.Invoke(null, new object?[] { "Hello World!", typeof(string), null });
            return resultDynamic!.ToString() ?? string.Empty;
        }
    }
}
