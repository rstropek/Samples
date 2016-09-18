using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace myApp
{
    public class Program
    {
        public static void Main(string[] args) =>
            new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
    
    public class Startup
    {
        // Note constructor injection here
        // (list of services see https://docs.asp.net/en/latest/fundamentals/startup.html#services-available-in-startup) 
        public Startup(IHostingEnvironment env)
        {
            Console.WriteLine($"App name: {env.ApplicationName}");
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("In ConfigureServices ...");
        }

        public void Configure(IApplicationBuilder app)
        {
            Console.WriteLine("In Configure ...");
            
            // Build pipeline
            
            app.Map("/beautiful", beautifulApp => beautifulApp.Run(
                async context => await context.Response.WriteAsync("Hello beautiful world!")));

            app.Run(async context => {
                await context.Response.WriteAsync("Hello world");
            });
        }
    }
}
