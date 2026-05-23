using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Data;

public class Person
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}

public class Order
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = null!; // e.g. "ORD-2025-000001"
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "EUR";
    public string Country { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Status { get; set; } = null!; // "Pending", "Paid", "Shipped", "Cancelled"
    
}

public class ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : DbContext(options)
{
    public DbSet<Person> People => Set<Person>();
    public DbSet<Order> Orders => Set<Order>();
}

public class ApplicationDataContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
{
    public ApplicationDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();

        var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false);
        var configuration = configurationBuilder.Build();

        optionsBuilder.UseSqlite(configuration.GetConnectionString("LinqDemo"))
            .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information)
            .EnableSensitiveDataLogging(); // Optional: shows parameter values

        return new ApplicationDataContext(optionsBuilder.Options);
    }
}