using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace WebApi;

public class Message
{
    public int Id { get; set; }
    public string Text { get; set; } = string.Empty;
}

public partial class ApplicationDataContext(DbContextOptions<ApplicationDataContext> options) : DbContext(options)
{
    public DbSet<Message> Messages => Set<Message>();
}

public class ApplicationDataContextFactory : IDesignTimeDbContextFactory<ApplicationDataContext>
{
    public ApplicationDataContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDataContext>();

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        var path = configuration["Database:path"] ?? throw new InvalidOperationException("Database path not configured.");
        var fileName = configuration["Database:fileName"] ?? throw new InvalidOperationException("Database file name not configured.");
        optionsBuilder.UseSqlite($"Data Source={path}/{fileName}");

        return new ApplicationDataContext(optionsBuilder.Options);
    }
}