using Fields;

var user = new User
{
    Email = "test@test.com",
    Age = 20
};

Console.WriteLine(user.Email);
Console.WriteLine(user.Age);

var dataProcessor = new DataProcessor
{
    RawData = [ 1, 2, 3, 4, 5 ]
};

Console.WriteLine(dataProcessor.Average);
Console.WriteLine(dataProcessor.Average);

var product = new Product
{
    Name = "  test product  ",
    Price = 10.946m
};

Console.WriteLine(product.Name);
Console.WriteLine(product.Price);

var customer = new ObservableCustomer
{
    Name = "John Doe"
};

customer.PropertyChanged += (sender, e) =>
{
    Console.WriteLine($"Property {e.PropertyName} changed");
};

customer.Name = "Jane Doe";
Console.WriteLine(customer.Name);