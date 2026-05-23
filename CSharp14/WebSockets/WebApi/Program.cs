using Microsoft.AspNetCore.WebSockets;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.AddSqliteDbContext<ApplicationDataContext>("database");
builder.AddServiceDefaults();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddWebSockets(options => { });

var app = builder.Build();

app.UseCors();
app.UseHttpsRedirection();
app.UseWebSockets();
app.MapOpenApi();
app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "v1"));
app.MapWebSocketEndpoints();

app.Run();
