#define WITH_CONTEXT

using System.Text.Json;
#if WITH_CONTEXT
using System.Text.Json.Serialization;
#endif

const string data = """
    {
        "people": [
            {
                "firstName": "John",
                "lastName": "Smith"
            },
            {
                "firstName": "Jane",
                "lastName": "Smith"
            }       
        ]
    }
    """;

#if !WITH_CONTEXT
var opt = new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
};
#endif

#if !WITH_CONTEXT
var f = JsonSerializer.Deserialize<Family>(data, opt);
var res = JsonSerializer.Serialize(f, opt);
#else
var f = JsonSerializer.Deserialize(data, FamilyDeserializationContext.Default.Family)!;
var res = JsonSerializer.Serialize(f, FamilyDeserializationContext.Default.Family);
#endif

Console.WriteLine(res);

#if WITH_CONTEXT
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(Family))]
internal partial class FamilyDeserializationContext : JsonSerializerContext
{
}
#endif

#region Types
class Person
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

class Family
{
    public List<Person> People { get; set; } = new();
}
#endregion
