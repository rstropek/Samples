using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Security;
using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public class KeyVaultReader : IKeyVaultReader
    {
        private KeyVaultOptions keyVaultOptions;
        private Dictionary<string, DateTime> lastRefreshTimestamps = new Dictionary<string, DateTime>();
        private Dictionary<string, string> secrets = new Dictionary<string, string>();

        public bool IsAvailable => keyVaultOptions?.VaultName != null;

        public KeyVaultReader(IOptions<KeyVaultOptions> keyVaultOptions)
        {
            this.keyVaultOptions = keyVaultOptions.Value;
        }

        private async Task<string> GetTokenAsync(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(keyVaultOptions.ClientID, keyVaultOptions.ClientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);
            return result.AccessToken;
        }

        public async Task<string> GetSecretAsync(string key)
        {
            var now = DateTime.UtcNow;
            if (lastRefreshTimestamps.ContainsKey(key) && lastRefreshTimestamps[key] > now.AddHours(-1))
            {
                return secrets[key];
            }
            else
            {
                var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetTokenAsync));
                var sec = await kv.GetSecretAsync($"https://{keyVaultOptions.VaultName}.vault.azure.net/secrets/{key}");

                lastRefreshTimestamps[key] = now;
                secrets[key] = sec.Value;

                return sec.Value;
            }
        }
    }
}
