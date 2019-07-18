using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace myApp
{
    public class Program
    {
        public static void Main(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build()
                .Run();
    }
    
    public class Startup
    {
        // Note constructor injection here
        // (list of services see https://docs.microsoft.com/en-us/aspnet/core/fundamentals/startup#services-available-in-startup) 
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
