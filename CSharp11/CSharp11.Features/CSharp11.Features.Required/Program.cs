using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

#region Basics
// The following line will not work because Age initialization is missing
//var p1 = new Person("Foo", "Bar");
var p1 = new Person("Foo", "Bar") { Age = 42 };
Console.WriteLine(p1);

var p2 = new CPerson("Foo,Bar,42");
Console.WriteLine(p2);
#endregion

#region Initialization
// Read data from DB
var dbp = new DatabasePerson
{ 
    FirstName = "Foo", 
    LastName = "Bar", 
    Age = 42,
    MiddleName = "Baz"
};

// Transfer DB data into DTO for web API
var webp = new WebResponsePerson(dbp.FirstName, dbp.LastName)
{
    Age = dbp.Age,
    MiddleName = dbp.MiddleName, // We cannot forget this line because of required
};
#endregion

#region AutoMapper
var config = new MapperConfiguration(
    cfg => cfg.CreateMap<DatabasePerson, WebResponsePerson>());
var mapper = config.CreateMapper();

// Automapper will NOT throw if required members are missing!
var webp2 = mapper.Map<WebResponsePerson>(dbp);

Console.WriteLine(JsonSerializer.Serialize(webp2));
#endregion

#region Basics (Data Types)
record Person(string FirstName, string LastName)
{
    // Note: Record properties will be added to default ToString impl.
    // Note: Required members need to be setable (set or init).
    public required int Age { get; init; }
}

class CPerson
{
    // Note: Constructor is marked to set required members
    [SetsRequiredMembers]
    public CPerson(string personData)
    {
        var pd = personData.AsSpan();
        var idx = pd.IndexOf(',');
        if (idx == -1) { throw new ArgumentException(null, nameof(personData)); }
        FirstName = pd[..idx].ToString();

        pd = pd[(idx + 1)..];
        idx = pd.IndexOf(',');
        if (idx == -1) { throw new ArgumentException(null, nameof(personData)); }
        LastName = pd[..idx].ToString();

        pd = pd[(idx + 1)..];
        Age = int.Parse(pd.ToString());
    }

    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required int Age { get; init; }

    public override string ToString() => $"{FirstName} {LastName} ({Age})";
}
#endregion

#region Initialization (Data Types)

class DatabasePerson
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public int Age { get; set; }
    
    public string? MiddleName { get; set; }
}

record WebResponsePerson(string FirstName, string LastName)
{
    public required int Age { get; init; }
    
    public required string? MiddleName { get; set; }
}
#endregion