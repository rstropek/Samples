using System;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace myApp
{
    
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MySingletonService>();
            services.AddTransient<MyTransientService>();
            services.AddScoped<MyScopedService>();
            
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseMvc();
        }
    }
    
    public class MyService
    {
        public MyService(string name) { this.Name = name; }
        public string Name { get; }
    }
    
    public class MySingletonService : MyService { public MySingletonService() : base("Singleton") {} }
    public class MyTransientService : MyService { public MyTransientService() : base("Transient") {} }
    public class MyScopedService : MyService { public MyScopedService() : base("Scoped") {} }
    
    [Route("greet")]
    public class GreetingController : Controller
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
}
