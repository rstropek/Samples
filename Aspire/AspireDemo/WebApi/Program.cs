using Microsoft.Data.SqlClient;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddSqlServerClient("sqldb");
builder.Services.AddHttpClient("multiplierapi", client =>
{
    client.BaseAddress = new("https://multiplierapi");
});

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseHttpsRedirection();

app.MapGet("/ping", () => "pong");
app.MapGet("/customers", () => Results.Ok(new[]
{
    new Customer(1, "John Doe", "john@doe.com"),
    new Customer(2, "Jane Doe", "jane@doe.com"),
}));
app.MapGet("/db", async (SqlConnection client, IHttpClientFactory factory) =>
{
    var csb = new SqlConnectionStringBuilder(client.ConnectionString) { InitialCatalog = "master" };
    using (var masterConn = new SqlConnection(csb.ConnectionString))
    {
        await masterConn.OpenAsync();
        using var createDbIfNotExistsCmd = masterConn.CreateCommand();
        createDbIfNotExistsCmd.CommandText = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'sqldb') CREATE DATABASE sqldb";
        await createDbIfNotExistsCmd.ExecuteNonQueryAsync();
    }

    await client.OpenAsync();
    using var cmd = client.CreateCommand();
    cmd.CommandText = "SELECT 42";
    int dbResult = (int)(await cmd.ExecuteScalarAsync())!;
    var multiplierClient = factory.CreateClient("multiplierapi");
    var multiplierResponse = await multiplierClient.PostAsJsonAsync("/multiply", new { ValueToMulitply = dbResult });
    var multiplierResult = await multiplierResponse.Content.ReadFromJsonAsync<OutputDto>();
    return new { Result = multiplierResult!.MultipliedValue };
});
app.MapGet("/environment", () =>
{
    List<EnvironmentVariable> result = [];

    foreach (var key in Environment.GetEnvironmentVariables().Keys)
    {
        result.Add(new EnvironmentVariable(key.ToString()!, Environment.GetEnvironmentVariable(key.ToString()!)!));
    }

    return Results.Ok(result);
});

app.Run();

record Customer(int ID, string Name, string Email);
record EnvironmentVariable(string Name, string Value);
record OutputDto(int MultipliedValue);
