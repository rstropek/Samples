using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

/*
 * We alway prefer AAD over handling secrets.
 * If we cannot avoid them:
 *   * We always prefer Key Vault over plain-text config files/settings.
 */

var app = WebApplication.Create(args);
app.UseDeveloperExceptionPage();

app.MapGet("/token", GetToken);
app.MapGet("/secrets", GetSecrets);
app.MapGet("/dpapi", DataProtectionApi);
app.MapGet("/setting", GetKeyVaultSetting);

async Task GetToken(HttpContext http)
{
    // Use default credentials. Will get token from
    // - Visual Studio
    // - Visual Studio Code
    // - Azure CLI
    // Read more at https://docs.microsoft.com/en-us/dotnet/api/overview/azure/identity-readme?view=azure-dotnet#defaultazurecredential
    // Note that you can specifically select the token source you want.
    var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        VisualStudioTenantId = app.Configuration["AAD:TenantId"]
    });

    // Use Managed Identity to get a token for Key Vault.
    var context = new TokenRequestContext(new[] { "https://storage.azure.com/.default" });
    var token = await credentials.GetTokenAsync(context);

    // Decode the token and return payload
    var tokenHandler = new JwtSecurityTokenHandler();
    var jwtToken = tokenHandler.ReadJwtToken(token.Token);
    http.Response.ContentType = MediaTypeNames.Application.Json;
    await http.Response.WriteAsJsonAsync(jwtToken.Payload);

    // Try creating an Ubuntu VM and retrieving the token on it:
    /*
     * sudo apt-get install -y jwt-cli libxml2-utils
     * 
     * curl 'http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https%3A%2F%2Fstorage.azure.com%2F' -H Metadata:true -s | jq .access_token -r
     * token=$(curl 'http://169.254.169.254/metadata/identity/oauth2/token?api-version=2018-02-01&resource=https%3A%2F%2Fstorage.azure.com%2F' -H Metadata:true -s | jq .access_token -r)
     * jwt $token
     * echo $(curl https://stkeys.blob.core.windows.net/keys/key.dat -H "x-ms-version: 2017-11-09" -H "Authorization: Bearer $token" -s) | xmllint --format -
    */
}

async Task GetSecrets(HttpContext http)
{
    // Create a Key Vault client with Managed Identity.
    var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        VisualStudioTenantId = app.Configuration["AAD:TenantId"]
    });
    var client = new SecretClient(new Uri("https://kv-mysecrets.vault.azure.net/"), credentials);

    // For debugging purposes, create an inventory of all secrets.
    // Note that in pratice you MUST NEVER EXPOSE YOUR SECRETS this way!
    var secrets = new List<Secret>();
    await foreach (var kvSecretProps in client.GetPropertiesOfSecretsAsync())
    {
        var secret = new Secret(kvSecretProps.Name);
        secrets.Add(secret);
        await foreach (var kvVersion in client.GetPropertiesOfSecretVersionsAsync(kvSecretProps.Name))
        {
            var kvSecret = await client.GetSecretAsync(kvSecretProps.Name, kvVersion.Version);
            secret.Values.Add(new(kvVersion.Version, kvSecret.Value.Value));
        }
    }

    // Return data about secrets.
    http.Response.StatusCode = 200;
    http.Response.ContentType = MediaTypeNames.Application.Json;
    await http.Response.WriteAsJsonAsync(secrets);
}

async Task GetKeyVaultSetting(HttpContext http)
{
    // You have various options to get settings from Key Vault.
    // Read more at https://docs.microsoft.com/en-us/azure/app-service/app-service-key-vault-references

    http.Response.StatusCode = 200;
    http.Response.ContentType = MediaTypeNames.Text.Plain;
    await http.Response.WriteAsync(app.Configuration["KeyVaultSetting"]);
}

async Task DataProtectionApi(HttpContext http)
{
    // Create blob storage to store key ring.
    // Read more at https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/introduction
    var credentials = new DefaultAzureCredential(new DefaultAzureCredentialOptions
    {
        VisualStudioTenantId = app.Configuration["AAD:TenantId"]
    });
    var blobClient = new BlobClient(new Uri("https://stkeys.blob.core.windows.net/keys/key.dat"), credentials);

    // Configure Data Protection API to use Key Vault
    var serviceCollection = new ServiceCollection();
    serviceCollection.AddDataProtection()
        .PersistKeysToAzureBlobStorage(blobClient)
        .ProtectKeysWithAzureKeyVault(new Uri("https://kv-mysecrets.vault.azure.net/keys/keys"), credentials);
    var services = serviceCollection.BuildServiceProvider();

    // Protect and unprotect a secret
    var protectorProvider = services.GetService<IDataProtectionProvider>();
    var protector = protectorProvider!.CreateProtector("Demo");
    var cypher = protector!.Protect("This is a secret!");
    var plain = protector!.Unprotect(cypher);

    http.Response.StatusCode = 200;
    http.Response.ContentType = MediaTypeNames.Application.Json;
    await http.Response.WriteAsJsonAsync(new { Cypher = cypher, Plaintext = plain });
}

await app.RunAsync();

record SecretValue(string Version, string Value);
record Secret(string Name) { public List<SecretValue> Values { get; init; } = new(); }
