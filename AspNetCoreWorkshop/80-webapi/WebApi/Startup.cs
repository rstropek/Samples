using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace WebApiDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add database context
            // Generate migrations with: dotnet ef migrations add ...
            // Update database with: dotnet ef database update 
            services.AddDbContext<OrderManagementContext>(options => options.UseSqlServer(
                Configuration["ConnectionStrings:DefaultConnection"]));

            // Add Open API generation using NSwag
            // See also https://github.com/RicoSuter/NSwag/wiki/AspNetCore-Middleware
            services.AddOpenApiDocument(document =>
            {
                document.DocumentName = "v3";
                document.Description = "This is a demo API";
                document.Title = "Demo API";
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // Add handler for unhandled exceptions
                app.UseExceptionHandler(errorApp =>
                {
                    errorApp.Run(context =>
                    {
                        // Use ProblemDetails (see RFC 7807 https://tools.ietf.org/html/rfc7807).
                        var problemDetails = new ProblemDetails
                        {
                            Title = "An unexpected error occurred!",
                            Status = 500,
                            Detail = "If this problem persists, contact support",
                            Instance = $"{context.TraceIdentifier}"
                        };

                        context.Response.StatusCode = 500;
                        context.Response.WriteJson(problemDetails, "application/problem+json");

                        return Task.CompletedTask;
                    });
                });

                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Add Open API UI
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseMvc();
        }
    }
}
