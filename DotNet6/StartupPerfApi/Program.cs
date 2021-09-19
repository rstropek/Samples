#pragma warning disable CA1050 // Declare types in namespaces

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Tracing;

// Write event trace when configuration process starts
AppEventSource.Log.ConfigureStarting();

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AddressBookContext>(options => options.UseInMemoryDatabase("AddressBook"));
builder.Services.AddControllers();
var app = builder.Build();

// For demo purposes, do something with EFCore
using (var scope = app.Services.CreateScope())
using (var context = scope.ServiceProvider.GetService<AddressBookContext>())
{
    context!.Database.EnsureCreated();
    context.Persons.Add(new() { FirstName = "Foo", LastName = "Bar" });
    await context.SaveChangesAsync();
}

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

// Write event trace when Configure has been finished
AppEventSource.Log.ConfigureFinished();

// Read more about how to use event traces for analyzing startup
// performance with PerfView at
// https://docs.microsoft.com/en-us/windows/uwp/dotnet-native/measuring-startup-improvement-with-net-native

app.Run();

[EventSource(Name = "StartupPerf")]
public sealed class AppEventSource : EventSource
{
	public static readonly AppEventSource Log = new();

	[Event(1)]
	public void ConfigureStarting() { WriteEvent(1, ""); }

	[Event(2)]
	public void ConfigureFinished() { WriteEvent(2, ""); }
}

public class Person
{
    public int ID { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class AddressBookContext : DbContext
{
    public AddressBookContext(DbContextOptions<AddressBookContext> options) : base(options)
    {
    }

    public DbSet<Person> Persons => Set<Person>();
}

#region Just the usual demo controller
[ApiController]
[Route("[controller]")]
public class PeopleController : ControllerBase
{
    private AddressBookContext context;

    public PeopleController(AddressBookContext context)
	{
        this.context = context;
    }

	[HttpGet]
	public ActionResult<Person> Get() => Ok(context.Persons);
}
#endregion
