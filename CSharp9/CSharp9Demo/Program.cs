using AutoMapper;
using CSharp9Demo;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NBattleshipCodingContest;
using System.Reflection;
using System.Threading.Tasks;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder => webBuilder
        .ConfigureServices(services =>
        {
            services
                .AddAuthentication("MyScheme")
                .AddScheme<DummyAuthenticationOptions, DummyAuthenticationHandler>("MyScheme", _ => { });
            services.AddAuthorization();
            services.AddControllers();
            services.AddSingleton<HeroRepository>();
            services.AddAutoMapper(config => config.CreateMap<Hero, HeroController.HeroShortDto>());
        })
        .Configure((context, app) =>
        {
            if (context.HostingEnvironment.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();

                static async Task SimpleAliveMessage(HttpContext context) => await context.Response.WriteAsync("I am alive");
                endpoints.MapGet("health", SimpleAliveMessage);

                [Authorize(AuthenticationSchemes = "MyScheme", Roles = "Admin")]
                static async Task AdminsOnlyAliveMessage(HttpContext context)
                {
                    var assemblyName = Assembly.GetExecutingAssembly().GetName();
                    await context.Response.WriteAsJsonAsync(new
                    {
                        Status = "Alive",
                        Assembly = assemblyName.FullName,
                        assemblyName.Version
                    });
                }
                endpoints.MapGet("health-secure", AdminsOnlyAliveMessage);
            });
        })
    )
    .Build()
    .Run();
