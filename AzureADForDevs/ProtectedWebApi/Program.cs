using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
builder.Services.AddAuthorization();
builder.Services.AddSingleton(new BlobServiceClient(
    new Uri($"https://{builder.Configuration["StorageAccountName"]}.blob.core.windows.net"),
    new DefaultAzureCredential()));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/ping", (HttpContext httpContext) =>
{
    return $"Pong, {httpContext!.User!.Identity!.Name}";
}).RequireAuthorization();

app.MapGet("/blob", async (BlobServiceClient client) =>
{
    var container = client.GetBlobContainerClient("data");
    BlobDownloadResult content = await container.GetBlobClient("data.txt").DownloadContentAsync();
    return content.Content.ToString();
});

app.Run();
