using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SignalRDrawingServer.Hubs;
using SignalRIntroduction;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<MathAlgorithms>();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddCors();
var app = builder.Build();

app.UseCors(builder => builder.WithOrigins("http://localhost:5000", "http://127.0.0.1:5000").AllowAnyHeader().AllowAnyMethod().AllowCredentials());
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.MapHub<GreetingHub>("/hub");
app.Run();
