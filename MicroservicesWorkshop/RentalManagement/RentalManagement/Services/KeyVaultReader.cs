using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public class KeyVaultReader : IKeyVaultReader
    {
        private KeyVaultOptions keyVaultOptions;

        public KeyVaultReader(IOptions<KeyVaultOptions> keyVaultOptions)
        {
            this.keyVaultOptions = keyVaultOptions.Value;
        }

        private async Task<string> GetTokenAsync(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(this.keyVaultOptions.ClientID, this.keyVaultOptions.ClientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);
            return result.AccessToken;
        }

        public async Task<string> GetSecretAsync(string key)
        {
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetTokenAsync));
            var sec = await kv.GetSecretAsync($"https://{this.keyVaultOptions.VaultName}.vault.azure.net/secrets/{key}");
            return sec.Value;
        }
    }
}
