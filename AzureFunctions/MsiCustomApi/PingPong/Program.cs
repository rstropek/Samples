using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Identity.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web.Resource;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.ConfigureServices((context, services) =>
        {
            services.AddMicrosoftIdentityWebApiAuthentication(context.Configuration);
            services.AddAuthentication();
            services.AddAuthorization();
        })
        .Configure(app =>
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                static async Task Anonymous(HttpContext context) =>
                    await context.Response.WriteAsync(JsonSerializer.Serialize("public from backend"));
                endpoints.MapGet("/anonymous", Anonymous);

                [Authorize]
                static async Task SignedInOnly(HttpContext context)
                {
                    context.ValidateAppRole("BackendAPIAccess");
                    await context.Response.WriteAsync(JsonSerializer.Serialize("private from backend"));
                }
                endpoints.MapGet("/signed-in-only", SignedInOnly);

            });

            app.Run(async context => await context.Response.WriteAsync("Pong"));
        });
    })
    .Build()
    .Run();
