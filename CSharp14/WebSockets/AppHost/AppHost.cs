var builder = DistributedApplication.CreateBuilder(args);

var sqlite = builder.AddSqlite(
    "database",
    builder.Configuration["Database:path"],
    builder.Configuration["Database:fileName"])
    .WithSqliteWeb(); // optionally add web admin UI (requires Docker/podman)

var webapi = builder.AddProject<Projects.WebApi>("webapi")
    .WithEnvironment("ConnectionStrings__Database", $"Data Source={Path.Combine(
        builder.Configuration["Database:path"]!, 
        builder.Configuration["Database:fileName"]!)}");

// Aspire 13 has been fundamentally redesigned in the context of .NET 10.
// Example: Integration of JavaScript is now more complete than before.
// Here we use the new Vite integration to add a frontend project based
// on Vite (https://vitejs.dev/). Other new integrations include
// e.g. Python-based projects.
var frontend = builder.AddViteApp("frontend", "../Frontend")
    .WithReference(webapi)
    .WithExternalHttpEndpoints();

builder.Build().Run();
