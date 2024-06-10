var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .PublishAsAzureSqlDatabase();
var sqldb = sql.AddDatabase("sqldb");

var multiplier = builder.AddProject<Projects.MultiplierApi>("multiplierapi")
    .WithReplicas(2);

var webapi = builder.AddProject<Projects.WebApi>("webapi")
    .WithReference(sqldb)
    .WithReference(multiplier);

builder.AddNpmApp("angular", "../Frontend")
    .WithReference(webapi)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

builder.Build().Run();
