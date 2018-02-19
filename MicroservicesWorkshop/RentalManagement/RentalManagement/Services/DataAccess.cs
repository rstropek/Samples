using Microsoft.Extensions.Options;
using System.Data.SqlClient;
using System.Text;
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

        public async Task<string> GetConnectionString()
        {
            var builder = new SqlConnectionStringBuilder();

            #region Build data source
            var dataSouceBuilder = new StringBuilder(dbServerOptions.Server.Length + 10);
            if (dbServerOptions.Protocol != null)
            {
                // Add protocol if configured
                dataSouceBuilder.Append(dbServerOptions.Protocol);
                dataSouceBuilder.Append(':');
            }

            dataSouceBuilder.Append(dbServerOptions.Server);
            if (dbServerOptions?.Protocol == "tcp")
            {
                // Add port if configured
                dataSouceBuilder.Append(',');
                dataSouceBuilder.Append(dbServerOptions.TcpPort ?? 1433);
            }
            builder.DataSource = dataSouceBuilder.ToString();

            builder.InitialCatalog = dbServerOptions.Database;
            builder.UserID = dbServerOptions?.DbUser ?? "demo";
            #endregion

            if (keyVaultReader != null && keyVaultReader.IsAvailable)
            {
                builder.Password = await keyVaultReader.GetSecretAsync("DbPassword");
            }
            else
            {
                builder.Password = dbServerOptions.DbPassword;
            }

            return builder.ToString();
        }
    }
}
