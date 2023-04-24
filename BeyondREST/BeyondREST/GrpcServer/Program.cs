using GrpcServer.Services;
using GrpcServer;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MathAlgorithms>();
builder.Services.AddGrpc();
builder.Services.AddGrpcReflection();
builder.Services.AddCors(o => o.AddPolicy("AllowAll", builder =>
{
    builder.AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Grpc-Status", "Grpc-Message");
}));

var app = builder.Build();

IWebHostEnvironment env = app.Environment;
if (env.IsDevelopment()) { app.MapGrpcReflectionService(); }

app.UseGrpcWeb();
app.UseCors();

app.MapGrpcService<GreeterService>().EnableGrpcWeb().RequireCors("AllowAll");
app.MapGrpcService<MathGuruService>().EnableGrpcWeb().RequireCors("AllowAll");

app.MapGet("/", async context =>
{
    await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
});

app.Run();
