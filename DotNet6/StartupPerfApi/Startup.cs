using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace StartupPerfApi
{
	public class Startup
	{
		public Startup(IConfiguration configuration) => Configuration = configuration;

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<AddressBookContext>(options => options.UseInMemoryDatabase("AddressBook"));
			services.AddControllers();
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			// Write event trace when Configure starts
			AppEventSource.Log.ConfigureStarting();

			// For demo purposes, do something with EFCore
			using (var scope = app.ApplicationServices.CreateScope())
			using (var context = scope.ServiceProvider.GetService<AddressBookContext>())
			{
				context.Database.EnsureCreated();
				context.Persons.Add(new() { FirstName = "Foo", LastName = "Bar" });
			}

			app.UseDeveloperExceptionPage();
			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthorization();
			app.UseEndpoints(endpoints => endpoints.MapControllers());

			// Write event trace when Configure has been finished
			AppEventSource.Log.ConfigureFinished();

			// Read more about how to use event traces for analyzing startup
			// performance with PerfView at
			// https://docs.microsoft.com/en-us/windows/uwp/dotnet-native/measuring-startup-improvement-with-net-native
		}
	}
}
