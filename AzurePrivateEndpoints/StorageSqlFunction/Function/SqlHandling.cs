using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Function
{
    public class SqlHandling
    {
        private readonly ILogger<BlobHandling> logger;

        public record GetQueryResult(string? QueryResult = null, string? IPAddresses = null, string? ErrorMessage = null);

        public SqlHandling(ILogger<BlobHandling> logger)
        {
            this.logger = logger;
        }

        [Function(nameof(GetQuery))]
        public async Task<HttpResponseData> GetQuery([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req)
        {
            var resultDto = new GetQueryResult();

            try
            {
                var connectionString = Environment.GetEnvironmentVariable("SqlConnection")!;

                var hostname = connectionString[7..connectionString.IndexOf(';')];
                logger.LogInformation($"Resolving hostname {hostname}");
                var ip = DnsUtil.ResolveDnsName(hostname);
                resultDto = resultDto with { IPAddresses = ip };

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();
                using var cmd = connection.CreateCommand();
                cmd.CommandText = "SELECT 42 AS ANSWER";
                var dbAnswer = await cmd.ExecuteScalarAsync();
                resultDto = resultDto with { QueryResult = dbAnswer!.ToString() };
            }
            catch (Exception ex)
            {
                resultDto = resultDto with { ErrorMessage = ex.Message };
            }

            var response = req.CreateResponse();
            await response.WriteAsJsonAsync(resultDto);
            return response;
        }
    }
}
