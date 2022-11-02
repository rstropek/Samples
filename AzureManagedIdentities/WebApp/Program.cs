using Azure.Core;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapGet("/ping", () => "pong");

app.MapGet("/storage-token", async () => $"{(await GetAccessToken("https://stuu3g75ep5thny.blob.core.windows.net"))[..100]}...");

app.Run();

static async Task<string> GetAccessToken(string scope)
{
    var tokenRequestResult = await new DefaultAzureCredential().GetTokenAsync(
        new TokenRequestContext(new[] { scope }));
    return tokenRequestResult.Token;
}