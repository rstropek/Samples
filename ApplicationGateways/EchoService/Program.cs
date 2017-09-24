using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace EchoService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateDefaultBuilder(args)
                .Configure(app =>
                {
                    app.Run(async (context) =>
                    {
                        var result = new StringBuilder();
                        result.Append($"Path: {context.Request.Path}\n");
                        result.Append($"Method: {context.Request.Method}\n");
                        result.Append($"QueryString: {context.Request.QueryString}\n");
                        result.Append("Headers:");
                        foreach (var header in context.Request.Headers)
                        {
                            result.Append($"  {header.Key}: {header.Value}\n");
                        }

                        context.Response.Headers.Add("Content-Type", "text/plain");
                        await context.Response.WriteAsync(result.ToString());
                    });
                })
                .Build()
                .Run();
        }
    }
}
