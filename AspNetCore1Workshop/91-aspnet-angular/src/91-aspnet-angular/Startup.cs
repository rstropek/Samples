using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AspNetCore1Angular2Intro.Controllers;
using AspNetCore1Angular2Intro.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCore1Angular2Intro
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Use the new configuration model here. Note the use of 
            // different configuration sources (json, env. variables,
            // middleware-specific configuration).
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();

            if (env.IsDevelopment())
            {
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Publish options read from configuration file. With that,
            // controllers can use ASP.NET DI to get options (see 
            // BooksController).
            services.Configure<BooksDemoDataOptions>(o =>
            {
                o.MinimumNumberOfBooks = Int32.Parse(this.Configuration.GetSection("booksDemoDataOptions:minimumNumberOfBooks").Value);
                o.MaximumNumberOfBooks = Int32.Parse(this.Configuration.GetSection("booksDemoDataOptions:maximumNumberOfBooks").Value);
            });

            // Add AI configuration
            services.AddApplicationInsightsTelemetry(this.Configuration);

            // Publish singleton for name generator
            services.AddSingleton(typeof(INameGenerator), typeof(NameGenerator));

            // Configure middlewares (CORS and MVC)
            services.AddCors();
            services.AddMvc();

            // Inject an implementation of ISwaggerProvider with defaulted settings applied
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Just for demo purposes throw an exception if query string contains "exception"
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.ContainsKey("exception"))
                {
                    throw new InvalidOperationException("Something bad happened ...");
                }

                await next();
            });

            app.UseStaticFiles();
            app.UseMvc();

            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();

            // Enable middleware to serve swagger-ui assets (HTML, JS, CSS etc.)
            app.UseSwaggerUi(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
        }
    }
}
