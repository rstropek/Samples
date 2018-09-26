using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Azure.KeyVault;

namespace NetCoreDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SecretsController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<ActionResult> Get()
        {
            // For details about connection string see https://docs.microsoft.com/en-us/azure/key-vault/service-to-service-authentication#connection-string-support
            //var azureServiceTokenProvider = new AzureServiceTokenProvider("RunAs=Developer; DeveloperTool=VisualStudio");
            var azureServiceTokenProvider = new AzureServiceTokenProvider();

            // Get access token for Key Vault
            var token = await azureServiceTokenProvider.GetAccessTokenAsync("https://vault.azure.net", "022e4faf-c745-475a-be06-06b1e1c9e39d");

            // Use MSI to get a KeyVaultClient
            using (var kvClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback)))
            {
                var secret = await kvClient.GetSecretAsync("https://msi-basta-vault-live.vault.azure.net/secrets/API-Key");

                return Ok(new
                {
                    MsiEndpoint = Environment.GetEnvironmentVariable("MSI_ENDPOINT"),
                    MsiSecret = Environment.GetEnvironmentVariable("MSI_SECRET"),
                    SecretValue = secret.Value,
                    AccessToken = token
                });
            }
        }
    }
}
