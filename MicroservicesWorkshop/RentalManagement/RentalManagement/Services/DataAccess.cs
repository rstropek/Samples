using Microsoft.Azure.KeyVault;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public class DataAccess : IDataAccess
    {
        private KeyVaultOptions keyVaultOptions;
        private DbServerOptions dbServerOptions;

        public DataAccess(IOptions<KeyVaultOptions> keyVaultOptions, IOptions<DbServerOptions> dbServerOptions)
        {
            this.keyVaultOptions = keyVaultOptions.Value;
            this.dbServerOptions = dbServerOptions.Value;
        }

        public async Task<string> GetDbNameAsync()
        {
            using (var conn = new SqlConnection(await GetConnectionString()))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT DB_NAME()";
                    return (string)await cmd.ExecuteScalarAsync();
                }
            }
        }

        private async Task<string> GetConnectionString()
        {
            var dbPassword = await GetSecretAsync();
            return $"Server=tcp:{dbServerOptions.Server},1433;Initial Catalog={dbServerOptions.Database};User ID=demo;Password={dbPassword}";
        }

        private async Task<string> GetTokenAsync(string authority, string resource, string scope)
        {
            var authContext = new AuthenticationContext(authority);
            var clientCred = new ClientCredential(this.keyVaultOptions.ClientID, this.keyVaultOptions.ClientSecret);
            var result = await authContext.AcquireTokenAsync(resource, clientCred);
            return result.AccessToken;
        }

        private async Task<string> GetSecretAsync()
        {
            var kv = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(GetTokenAsync));
            var sec = await kv.GetSecretAsync($"https://{this.keyVaultOptions.VaultName}.vault.azure.net/secrets/DbPassword");
            return sec.Value;
        }
    }
}
