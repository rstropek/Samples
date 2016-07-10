using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

namespace myApp
{
    public class Program
    {
        public static void Main(string[] args) =>
            new WebHostBuilder().UseKestrel().UseStartup<Startup>().Build().Run();
    }
    
    public class Startup
    {
        // Note constructor injection here
        // (list of services see https://docs.asp.net/en/latest/fundamentals/startup.html#services-available-in-startup) 
        public Startup(IHostingEnvironment env)
        {
            Console.WriteLine($"App name: {env.ApplicationName}");
            Console.WriteLine($"Root path: {env.WebRootPath}");
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("In ConfigureServices ...");
        }

        public void Configure(IApplicationBuilder app)
        {
            Console.WriteLine("In Configure ...");
            
            // Build pipeline
            
            app.Use(async (context, next) => {
                await context.Response.WriteAsync(">>> Hello ");
                await next();
                await context.Response.WriteAsync("World <<<");
            });
            
            app.Map("/beautiful", beautifulApp => beautifulApp.Run(
                async context => await context.Response.WriteAsync("beautiful ")));
        }
    }
}
