using System.Diagnostics;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using static System.Net.Mime.MediaTypeNames;

const string STORAGE_DOMAIN = "blob.core.windows.net";
const string KEY_VAULT_DOMAIN = "vault.azure.net";
const string SQL_DOMAIN = "database.windows.net";

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
var app = builder.Build();

app.UseExceptionHandler(exceptionHandlerApp =>
 {
     exceptionHandlerApp.Run(async context =>
     {
         var exceptionHandlerPathFeature =context.Features.Get<IExceptionHandlerPathFeature>();

         context.Response.StatusCode = StatusCodes.Status500InternalServerError;
         context.Response.ContentType = Text.Plain;
         if (exceptionHandlerPathFeature?.Error != null)
         {
             await context.Response.WriteAsync(exceptionHandlerPathFeature?.Error.Message ?? "No exception message.");
         }
         else
         {
             await context.Response.WriteAsync("An exception was thrown.");
         }
     });
 });

app.MapGet("/ping", () => "pong");

#region Azure Storage
app.MapGet("/storage-token", async (IConfiguration config) =>
{
    var accountName = config["AccountNames:Storage"] ?? "stau4rvl4is2cyy";
    return Results.Ok($"{(await GetAccessToken($"https://{accountName}.${STORAGE_DOMAIN}"))[..100]}...");
});

app.MapPost("/copy-file", async (string uri, string name, IHttpClientFactory httpClientFactory, IConfiguration config) =>
{
    // Download source file
    var client = httpClientFactory.CreateClient();
    var responseMsg = await client.GetAsync(uri);
    var sourceStream = await responseMsg.Content.ReadAsStreamAsync();

    // Create blob service client
    var accountName = config["AccountNames:Storage"] ?? "stau4rvl4is2cyy";
    var accountUri = new Uri($"https://{accountName}.{STORAGE_DOMAIN}");
    var credentials = new DefaultAzureCredential();
    var blobServiceClient = new BlobServiceClient(accountUri, credentials);

    // Create container if not exists
    var containerClient = blobServiceClient.GetBlobContainerClient("data");
    await containerClient.CreateIfNotExistsAsync();

    // Upload (if necessary replace) file into blob storage
    var blobClient = containerClient.GetBlobClient(name);
    await blobClient.DeleteIfExistsAsync();
    await blobClient.UploadAsync(sourceStream);
    await blobClient.SetHttpHeadersAsync(new()
    {
        ContentType = responseMsg.Content?.Headers?.ContentType?.ToString() ?? "image/png"
    });

    // Generate SAS url
    var userDelegationKey = await blobServiceClient.GetUserDelegationKeyAsync(
        DateTimeOffset.UtcNow, DateTimeOffset.UtcNow.AddMinutes(5));
    var sasBuilder = new BlobSasBuilder()
    {
        BlobContainerName = containerClient.Name,
        BlobName = blobClient.Name,
        Resource = "b",
        StartsOn = DateTimeOffset.UtcNow,
        ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(5)
    };
    sasBuilder.SetPermissions(BlobSasPermissions.Read);
    var sasParameters = sasBuilder.ToSasQueryParameters(userDelegationKey, blobServiceClient.AccountName);
    var blobUriBuilder = new BlobUriBuilder(blobClient.Uri) { Sas = sasParameters };

    // Return CREATED response with location header
    return Results.Created(blobUriBuilder.ToUri(), null);
});

app.MapGet("/raw-token", async (IHttpClientFactory httpClientFactory, IConfiguration config) =>
{
    var client = httpClientFactory.CreateClient();
    var accountName = config["AccountNames:Storage"] ?? "stau4rvl4is2cyy";
    var fullAccountName = $"{accountName}.{STORAGE_DOMAIN}";
    var requestUrl = $"http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https%3A%2F%2F{fullAccountName}%2F";
    var requestMsg = new HttpRequestMessage(HttpMethod.Get, requestUrl);
    requestMsg.Headers.Add("Metadata", "true");
    var responseMsg = await client.SendAsync(requestMsg);
    var response = await responseMsg.Content.ReadAsStringAsync();
    return Results.Ok(response);
});
#endregion

#region Azure Key Vault
static SecretClient GetSecretClient(IConfiguration config)
{
    var keyVaultName = config["AccountNames:KeyVault"] ?? "kv-sbwa7dm5lugos";
    var keyVaultUri = new Uri($"https://{keyVaultName}.{KEY_VAULT_DOMAIN}");
    var credentials = new DefaultAzureCredential();
    return new SecretClient(keyVaultUri, credentials);
}

app.MapPost("/keep-secret", async (string name, string value, IConfiguration config) =>
{
    var keyVaultClient = GetSecretClient(config);
    await keyVaultClient.SetSecretAsync(name, value);
    return Results.Created($"/get-secret?name={name}", null);
});

app.MapGet("/get-secret", async (string name, IConfiguration config) =>
{
    var keyVaultClient = GetSecretClient(config);
    var secret = await keyVaultClient.GetSecretAsync(name);
    return Results.Ok(new
    {
        Name = name,
        Value = secret.Value.Value
    });
});
#endregion

#region Azure SQL
app.MapGet("/from-db", async (IConfiguration config) =>
{
    var dbServerName = $"{config["AccountNames:DbServer"] ?? "sql-sbwa7dm5lugos"}.{SQL_DOMAIN}";
    var connectionString = $"Server={dbServerName}; Database={config["AccountName:Database"] ?? "sqldb-sbwa7dm5lugos"}";
    using var connection = new SqlConnection(connectionString);
    var token = await new DefaultAzureCredential().GetTokenAsync(new TokenRequestContext(new[] { "https://database.windows.net/.default" }));
    connection.AccessToken = token.Token;
    await connection.OpenAsync();

    using var command = new SqlCommand("SELECT 42 AS ANSWER", connection);
    int answer = (int)(await command.ExecuteScalarAsync())!;

    return Results.Ok(new { Answer = answer });
});
#endregion

#region Custom API
app.MapGet("/ping-backend", async (IHttpClientFactory httpClientFactory, IConfiguration config) =>
{
    var backendPingUri = $"https://{config["AccountNames:Backend"]}/api/ping";
    var client = httpClientFactory.CreateClient();

    return Results.Ok(await client.GetStringAsync(backendPingUri));
});

app.MapGet("/backend-token", async (IConfiguration config) =>
{
    var audience = config["AccountNames:Audience"];
    Debug.Assert(audience != null);
    var token = await GetAccessToken(audience);
    return Results.Ok(token);
});

app.MapGet("/secure-ping-backend", async (IHttpClientFactory httpClientFactory, IConfiguration config) =>
{
    try
    {
        var audience = config["AccountNames:Audience"];
        Debug.Assert(audience != null);
        var token = await GetAccessToken(audience);

        var backendPingUri = $"https://{config["AccountNames:Backend"]}/api/secureping";
        var client = httpClientFactory.CreateClient();
        var requestMsg = new HttpRequestMessage(HttpMethod.Get, backendPingUri);
        requestMsg.Headers.Add("Authorization", $"Bearer {token}");
        var responseMsg = await client.SendAsync(requestMsg);
        var response = await responseMsg.Content.ReadAsStringAsync();

        return Results.Content(response, "application/json");
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
});
#endregion

app.Run();

static async Task<string> GetAccessToken(string scope)
{
    var tokenRequestResult = await new DefaultAzureCredential()
        .GetTokenAsync(new TokenRequestContext(new[] { scope }));
    return tokenRequestResult.Token;
}