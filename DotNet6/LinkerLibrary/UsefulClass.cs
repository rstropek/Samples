#nullable enable
#pragma warning disable CA1822 // Mark members as static

using System.Reflection;
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

            // Get Serialize overload (https://docs.microsoft.com/en-us/dotnet/api/system.text.json.jsonserializer.serialize?view=net-5.0#System_Text_Json_JsonSerializer_Serialize_System_Object_System_Type_System_Text_Json_JsonSerializerOptions_)
            var mi = t!.GetMethods().First(m => m.Name == "Serialize" && !m.IsGenericMethod && m.GetParameters().Length == 3);
            var resultDynamic = mi!.Invoke(null, new object?[] { "Hello World!", typeof(string), null });
            return resultDynamic!.ToString() ?? string.Empty;
        }
    }
}
