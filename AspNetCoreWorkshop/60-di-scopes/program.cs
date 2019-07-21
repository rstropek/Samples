using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore;

namespace myApp
{
    public class Program
    {
        public static void Main(string[] args) =>
            WebHost.CreateDefaultBuilder(args).UseStartup<Startup>().Build().Run();
    }

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MySingletonService>();
            services.AddTransient<MyTransientService>();
            services.AddScoped<MyScopedService>();

            services.AddMvcCore();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseDiLogging();
            app.UseMvc();
        }
    }

    public class MyService
    {
        public MyService(string name) { this.Name = name; }
        public string Name { get; }
    }

    public class MySingletonService : MyService { public MySingletonService() : base("Singleton") { } }
    public class MyTransientService : MyService { public MyTransientService() : base("Transient") { } }
    public class MyScopedService : MyService { public MyScopedService() : base("Scoped") { } }

    [ApiController]
    [Route("greet")]
    public class GreetingController : ControllerBase
    {
        public GreetingController(MySingletonService s, MyTransientService t1, MyTransientService t2, MyScopedService sc1, MyScopedService sc2)
        {
            Console.WriteLine($@"Got
  Singleton: {s.GetHashCode()}
  Transient 1: {t1.GetHashCode()}
  Transient 2: {t2.GetHashCode()}
  Scoped 1: {sc1.GetHashCode()}
  Scoped 2: {sc2.GetHashCode()}");
        }

        public string Get()
        {
            return "Hello";
        }
    }

    public class DiLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public DiLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context, MySingletonService s, MyTransientService t1, MyTransientService t2, MyScopedService sc1, MyScopedService sc2)
        {
            Console.WriteLine($@"Custom middleware got
  Singleton: {s.GetHashCode()}
  Transient 1: {t1.GetHashCode()}
  Transient 2: {t2.GetHashCode()}
  Scoped 1: {sc1.GetHashCode()}
  Scoped 2: {sc2.GetHashCode()}");

            // Call the next delegate/middleware in the pipeline
            return this._next(context);
        }
    }

    public static class DiLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseDiLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<DiLoggingMiddleware>();
        }
    }
}
