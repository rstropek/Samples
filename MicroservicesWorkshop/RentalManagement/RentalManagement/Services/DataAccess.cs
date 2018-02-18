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
        private IKeyVaultReader keyVaultReader;
        private DbServerOptions dbServerOptions;

        public DataAccess(IKeyVaultReader keyVaultReader, IOptions<DbServerOptions> dbServerOptions)
        {
            this.keyVaultReader = keyVaultReader;
            this.dbServerOptions = dbServerOptions.Value;
        }

        public async Task<string> GetDbNameAsync()
        {
            // Note that in practice you should cache the connection string for a while because
            // getting it from key vault is not very fast.
            using (var conn = new SqlConnection(await GetConnectionString()))
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    // Imagine your DB access code here...

                    cmd.CommandText = "SELECT DB_NAME()";
                    return (string)await cmd.ExecuteScalarAsync();
                }
            }
        }

        private async Task<string> GetConnectionString()
        {
            var dbPassword = await keyVaultReader.GetSecretAsync("DbPassword");
            return $"Server=tcp:{dbServerOptions.Server},1433;Initial Catalog={dbServerOptions.Database};User ID=demo;Password={dbPassword}";
        }
    }
}
