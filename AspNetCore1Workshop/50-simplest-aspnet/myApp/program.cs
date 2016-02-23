using System;
using Microsoft.AspNet.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;

namespace myApp
{
    public class Program
    {
        // Note that we have a custom entry point here. It is used if you start
        // the app with "dnx assert". If you use "dnx web", the Main method
        // from Microsoft.AspNet.Server.Kestrel is used
        // (https://github.com/aspnet/KestrelHttpServer/blob/1.0.0-rc1/src/Microsoft.AspNet.Server.Kestrel/Program.cs)
        
        public void Main(string[] args)
        {
            // You could do e.g. some checks here ...
            Console.WriteLine("Checking configuration ...");
            if (1 == 0)
            {
                throw new InvalidOperationException("Configuration not ok ...");
            }
            
            // Now we can start the web app passing on the command line
            // arguments we received.
            Console.WriteLine("Configuration is ok, starting web app ...");
            WebApplication.Run<Startup>(args);
        }
    }
    
    public class Startup
    {
        // Note that when using "dnx web", startup class is detected based
        // on conventions (see https://github.com/aspnet/Hosting/blob/1.0.0-rc1/src/Microsoft.AspNet.Hosting/Startup/StartupLoader.cs#L41-L89).
        
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine("In ConfigureServices ...");
        }

        public void Configure(IApplicationBuilder app)
        {
            Console.WriteLine("In Configure ...");
            
            // Build pipeline
            
            app.Use(async (context, next) => {
                await context.Response.WriteAsync(">>>");
                await next();
            });
            
            app.Use(async (context, next) => {
                await context.Response.WriteAsync("Hello World!");
                await next();
            });
            
            app.Run(async (context) => await context.Response.WriteAsync("<<<"));
        }
    }
}
