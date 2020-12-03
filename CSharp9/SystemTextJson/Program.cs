using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

CircularReferences();
await HttpClientExtensionMethods();

static void CircularReferences()
{
    var research = new Department("R&D");
    var eve = new Employee("Eve", research);
    research.Employee.Add(eve);

    var options = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.Preserve,
        WriteIndented = true
    };

    // Try with and without options
    var json = JsonSerializer.Serialize(new[] { eve }, options);
    Console.WriteLine(json);

    // Note ability to deserialize objects with parameterized ctors.
    // Also note support for records.
    var employees = JsonSerializer.Deserialize<List<Employee>>(json);
    Console.WriteLine($"{employees![0].Name} works in {employees[0].Department.Name}");
}

static async Task HttpClientExtensionMethods()
{
    // New extension method makes it trivial to get/post JSON
    using HttpClient client = new() { BaseAddress = new Uri("https://pokeapi.co/api/v2/pokemon/") };
    var pokemon = await client.GetFromJsonAsync<Pokemon>("1");

    Console.WriteLine($"Pokemon {pokemon!.Id} is {pokemon.Name}");
}

record Employee(string Name, Department Department);

record Department(string Name)
{
    public List<Employee> Employee { get; } = new();
}

record Pokemon(string Name, int Id, int Weight);
