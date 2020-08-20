using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using GrpcServer.Services;
using GrpcServer;

Host.CreateDefaultBuilder(args)
.ConfigureWebHostDefaults(webBuilder =>
{
    webBuilder.ConfigureServices(services =>
    {
        services.AddSingleton<MathAlgorithms>();
        services.AddGrpc();

        services.AddCors(o => o.AddPolicy("AllowAll", builder =>
        {
            builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .WithExposedHeaders("Grpc-Status", "Grpc-Message");
        }));
    })
    .Configure((context, app) =>
    {
        if (context.HostingEnvironment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseGrpcWeb();
        app.UseCors();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
            endpoints.MapGrpcService<MathGuruService>().EnableGrpcWeb().RequireCors("AllowAll");

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    });
})
.Build()
.Run();
