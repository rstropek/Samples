using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ProtectedWebAPI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var domain = $"https://{Configuration["Auth0:Domain"]}/";
            services.AddJwtBearerAuthentication(o =>
            {
                o.Authority = domain;
                o.Audience = Configuration["Auth0:ApiIdentifier"];
            });

            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "read:data",
                    policy => policy.Requirements.Add(new HasScopeRequirement("read:data", domain)));
                options.AddPolicy(
                    "write:data",
                    policy => policy.Requirements.Add(new HasScopeRequirement("write:data", domain)));
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
