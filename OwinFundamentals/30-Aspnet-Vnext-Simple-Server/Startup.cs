using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using System;

namespace AspnetVnextSimpleServer
{
	public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.Run(async context =>
			{
				Console.WriteLine($"Got request for {context.Request.Path}");

				context.Response.ContentType = "text/html";
				context.Response.StatusCode = 200;

				await context.Response.WriteAsync(
					$@"<!DOCTYPE 'html'><html><body><h1>Hello from {
						context.Request.Path }!</h1></body></html>");
            });
        }
    }
}
