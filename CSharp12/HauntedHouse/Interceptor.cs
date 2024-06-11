using HauntedHouse;

namespace Interceptor;

[CompilerGenerated]
static class HauntedHouseParserAot
{
    // Note how we can intercept a method call from generated code.
    [InterceptsLocation("/root/github/Samples/CSharp12/HauntedHouse/Program.cs", line: 13, character: 23)]
    public static Dictionary<string, Room> GetHouseWithSourceGen(this HauntedHouseParser hhp, string json)
    {
        return new()
        {
            ["TestRoom"] = new("Test Room", "This is a test room for interceptors", RoomFeatures.Empty, [])
        };
    }
}