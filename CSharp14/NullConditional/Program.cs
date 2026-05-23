Person[] people =
[
    new() { Name = "John Doe", Weight = 42, Address = null },
    new() { Name = "Jane Doe", Weight = 42, Address = new() { Street = "456 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
    new() { Name = "Jim Doe", Weight = 42, Address = new() { Street = "789 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
    new() { Name = "Jill Doe", Weight = 42, Address = null },
    new() { Name = "Jack Doe", Weight = 42, Address = new() { Street = "123 Main St", City = "Anytown", State = "CA", Zip = "12345" } },
];

var p = people.FirstOrDefault(p => p.Name == "John Doe");
p?.Address?.Street = p.Address.Street.ToUpper(); // Note: null conditional on left side of assignment
Console.WriteLine($"\"{p?.Address?.Street}\"");

Person[]? ps = null;
ps?[0].Name = "John Doe";
Console.WriteLine($"\"{ps?[0]?.Name}\"");

Person? person = null;
// Somebody gained weight
person?.Weight += 10; // Note: null conditional on left side of compound assignment
// Note that null-conditional assignments to not work with ++ or --
// person?.Weight++;

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
    public decimal Weight { get; set; }
}

