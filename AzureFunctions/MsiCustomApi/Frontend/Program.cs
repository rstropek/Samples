using Azure.Core;
using Azure.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;

Host.CreateDefaultBuilder(args)
    .ConfigureWebHostDefaults(webBuilder =>
    {
        webBuilder.Configure(app =>
        {
            app.UseDeveloperExceptionPage();

            static async Task ReturnToken(HttpContext context, string scope)
            {
                var token = await GetAccessToken(scope);
                context.Response.Headers["Content-Type"] = "application/json";
                await context.Response.WriteAsync(JsonSerializer.Serialize(token));
            }

            app.MapWhen(context => context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/storage-token"),
                headersApp => headersApp.Run(async context => await ReturnToken(context, "https://storage.azure.com//.default")));
            app.MapWhen(context => context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/backend-token"),
                headersApp => headersApp.Run(async context => await ReturnToken(context, "api://backend-api.crm.rainertimecockpit.onmicrosoft.com")));

            static async Task GetPrivateContentFromBackend(HttpContext context, string path, bool withToken)
            {
                using var client = new HttpClient();
                if (withToken)
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetAccessToken("api://backend-api.crm.rainertimecockpit.onmicrosoft.com"));
                }

                var response = await client.GetAsync($"https://backend-api-demo.azurewebsites.net/{path}");
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();

                context.Response.Headers["Content-Type"] = "application/json";
                await context.Response.WriteAsync(content);
            }
            app.MapWhen(context => context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/backend"),
                headersApp => headersApp.Run(async context => await GetPrivateContentFromBackend(context, "signed-in-only", true)));
            app.MapWhen(context => context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/backend-without-token"),
                headersApp => headersApp.Run(async context => await GetPrivateContentFromBackend(context, "signed-in-only", false)));
            app.MapWhen(context => context.Request.Method == "GET" && context.Request.Path.StartsWithSegments("/backend-anonymous"),
                headersApp => headersApp.Run(async context => await GetPrivateContentFromBackend(context, "public", false)));
        });
    })
    .Build()
    .Run();


static async Task<string> GetAccessToken(string scope)
{
    var tokenRequestResult = await new DefaultAzureCredential().GetTokenAsync(
        new TokenRequestContext(new[] { scope }));
    return tokenRequestResult.Token;
}
