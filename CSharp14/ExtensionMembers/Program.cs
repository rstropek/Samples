//using StringExtensionsTraditional;
//using StringExtensions;
using MultipleStringExtensions;
using ExtensionEverything;
using StaticExtensions;
using TaskExtensions;

// Usage
string email = "user@example.com";
if (!email.IsValidEmail()) { Console.WriteLine("Invalid email"); }
if (!email.IsEmail) { Console.WriteLine("Invalid email"); }

string numberAsString = "12345";
numberAsString += "67";
Console.WriteLine(numberAsString);

string longText = "This is a very long text that needs truncation";
string truncated = longText.TruncateWithSuffix(20); // "This is a very lo..."
Console.WriteLine(truncated);

var batman = string.GenerateBatman(16);
Console.WriteLine(batman);

static async Task<int> SimulateDBAccess(TimeSpan duration)
{
    await Task.Delay(duration);
    return 42;
}

var dbResult = await SimulateDBAccess(TimeSpan.FromSeconds(1)).WithDefaultOnTimeout(TimeSpan.FromSeconds(0.5), 0);
Console.WriteLine(dbResult);

Task<int>[] dbTasks = [ SimulateDBAccess(TimeSpan.FromSeconds(0.25)), SimulateDBAccess(TimeSpan.FromSeconds(0.5)), SimulateDBAccess(TimeSpan.FromSeconds(1)) ];
var multiplier = 2;
var results = await dbTasks.WhenAllConverted(r => r * multiplier++);
Console.WriteLine(string.Join(", ", results));
