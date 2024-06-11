using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SignalRDrawingServer.Hubs;

var builder = WebApplication.CreateSlimBuilder(args);
builder.Services.AddSignalR();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(builder => builder.WithOrigins("http://localhost:1234", "http://127.0.0.1:1234").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseRouting();
app.MapHub<DrawingHub>("/hub");
app.Run();
