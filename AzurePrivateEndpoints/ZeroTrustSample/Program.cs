using System.Data.SqlClient;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;
using System.Linq;
using System.Net.Http;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure((webContext, app) =>
        {
            app.UseDeveloperExceptionPage();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                static async Task ShowHeaders(HttpContext context)
                {
                    context.Response.Headers["Content-Type"] = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(context.Request.Headers.ToList()));
                }
                endpoints.MapGet("/Headers", ShowHeaders);

                async Task RequestBackend(HttpContext context)
                {
                    using var client = new HttpClient();
                    var result = JsonSerializer.Deserialize<string>(await client.GetStringAsync($"{webContext.Configuration["BackendApi"]}/ping"));
                    await context.Response.WriteAsync(JsonSerializer.Serialize(result));
                }
                endpoints.MapGet("/RequestBackend", RequestBackend);

                async Task AccessDatabase(HttpContext context)
                {
                    await using var conn = new SqlConnection(webContext.Configuration["ConnectionStrings:Database"]);
                    if (bool.TryParse(webContext.Configuration["UseManagedIdentity"], out var useMsi) && useMsi)
                    {
                        conn.AccessToken = await GetSqlAccessToken();
                    }

                    await conn.OpenAsync();
                    using var cmd = conn.CreateCommand();
                    cmd.CommandText = "SELECT 42 AS ANSWER";
                    var answer = await cmd.ExecuteScalarAsync();

                    context.Response.Headers["Content-Type"] = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(answer));
                }
                endpoints.MapGet("/Database", AccessDatabase);

                async Task DnsResolution(HttpContext context)
                {
                    var addressesString = new StringBuilder();
                    var addresses = Dns.GetHostAddresses(webContext.Configuration["DnsHost"]);
                    context.Response.Headers["Content-Type"] = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(string.Join(", ", addresses.ToArray().Select(x => x.ToString()))));
                }
                endpoints.MapGet("/Dns", DnsResolution);
            });
        });
    })
    .Build().Run();

static async Task<string> GetSqlAccessToken()
{
    var tokenRequestResult = await new DefaultAzureCredential().GetTokenAsync(
        new TokenRequestContext(new[] { "https://database.windows.net//.default" }));
    return tokenRequestResult.Token;
}
