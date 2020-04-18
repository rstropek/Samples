using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Frontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                    webBuilder
                        .UseStartup<Startup>()
                        .UseUrls("http://+:" + Environment.GetEnvironmentVariable("PORT")))
                .Build()
                .Run();
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
        }
    }

    [ApiController]
    [Route("[controller]")]
    public class ApiController : ControllerBase
    {
        private readonly IConfiguration configuration;
        private readonly HttpClient httpClient;

        public ApiController(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            this.configuration = configuration;
            httpClient = httpClientFactory.CreateClient();
        }

        private class FooResult
        {
            [JsonPropertyName("foo")]
            public string Foo { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            string dbResult;
            var connectionString = configuration["SqlConnection"];
            try
            {
                using var conn = new SqlConnection(connectionString);
                await conn.OpenAsync();
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT 'bar' AS Foo";
                dbResult = (await cmd.ExecuteScalarAsync()) as string;
            }
            catch (Exception ex)
            {
                dbResult = $"{connectionString}: {ex.Message}";
            }

            var addressesString = new StringBuilder();
            var addresses = Dns.GetHostAddresses("ddosqlserver.database.windows.net");
            foreach (var hostAddress in addresses)
            {
                addressesString.AppendFormat("{0}\n", hostAddress);
            }

            string apiResult;
            var hostHeader = configuration["Backend:Host"];
            var address = configuration["Backend:Address"];
            try
            {
                using var request = new HttpRequestMessage();
                request.Headers.Add("Accept", "application/json");
                if (!string.IsNullOrEmpty(hostHeader))
                {
                    request.Headers.Host = hostHeader;
                }

                var uriBuilder = new UriBuilder
                {
                    Scheme = "http",
                    Host = address,
                    Path = "/foo"
                };
                request.RequestUri = uriBuilder.Uri;
                var response = await httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                apiResult = JsonSerializer.Deserialize<FooResult>(await response.Content.ReadAsStringAsync()).Foo;
            }
            catch (Exception ex)
            {
                apiResult = $"Access {address} using host header {hostHeader}: {ex.Message}";
            }

            return Ok(new { FooDB = dbResult, FooApi = apiResult, Dns = addressesString.ToString() });
        }
    }
}
