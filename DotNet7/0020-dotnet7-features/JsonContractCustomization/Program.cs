using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

var options = new JsonSerializerOptions
{
    TypeInfoResolver = new DefaultJsonTypeInfoResolver
    {
        Modifiers =
        {
            (JsonTypeInfo jsonTypeInfo) =>
            {
                if (jsonTypeInfo.Type != typeof(Person))
                {
                    return;
                }

                // Ignore middle name property during serialization
                var property = jsonTypeInfo.Properties.First(p => p.Name == nameof(Person.MiddleName));
                property.ShouldSerialize = (_, _) => false;

                // Allow reading age from string
                property = jsonTypeInfo.Properties.First(p => p.Name == nameof(Person.Age));
                property.NumberHandling = JsonNumberHandling.AllowReadingFromString;
            }
        }
    }
};

var json = JsonSerializer.Serialize(new Person(), options);
Console.WriteLine(json);

var p = JsonSerializer.Deserialize<Person>("""
    {
        "FirstName": "John",
        "LastName": "Smith",
        "Age": "42"
    }
    """, options);
Console.WriteLine(p.Age);

class Person
{
    public string FirstName { get; set; } = "Jane";
    public string? MiddleName { get; set; } = "Regina";
    public string LastName { get; set; } = "Doe";
    public int Age { get; set; } = 42;
}