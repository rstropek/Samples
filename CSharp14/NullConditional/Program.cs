Person[] people =
[
    new() { Name = "John Doe", Address = null },
    new() { Name = "Jane Doe", Address = new() { Street = "456 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
    new() { Name = "Jim Doe", Address = new() { Street = "789 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
    new() { Name = "Jill Doe", Address = null },
    new() { Name = "Jack Doe", Address = new() { Street = "123 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
];

var p = people.FirstOrDefault(p => p.Name == "John Doe");
p?.Address?.Street = p.Address.Street.ToUpper(); // Note: null conditional on left side of assignment
Console.WriteLine($"\"{p?.Address?.Street}\"");

Person[]? ps = null;
ps?[0].Name = "John Doe";
Console.WriteLine($"\"{ps?[0]?.Name}\"");

class Address
{
    public required string Street { get; set; }
    public required string City { get; set; }
    public required string State { get; set; }
    public required string Zip { get; set; }
}

class Person
{
    public required string Name { get; set; }
    public Address? Address { get; set; } = null;
}

