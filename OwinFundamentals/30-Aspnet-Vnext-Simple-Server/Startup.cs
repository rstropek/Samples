using Microsoft.AspNet.Builder;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.StaticFiles;
using Microsoft.Framework.DependencyInjection;
using System;
using System.IO;

namespace AspnetVnextSimpleServer
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseMvc();

			app.Map("/site", siteApp =>
			{
				siteApp.UseFileServer(new FileServerOptions
				{
					FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")),
					EnableDirectoryBrowsing = true
				});
			});

			app.Use(async (context, next) =>
			{
				await context.Response.WriteAsync("=== BEFORE ===");
				await next();
				await context.Response.WriteAsync("=== AFTER ===");
			});

			app.Run(async (context) =>
			{
				Console.WriteLine($"Got request for {context.Request.Path}");

				await context.Response.WriteAsync(
					$@"<!DOCTYPE 'html'><html><body><h1>Hello from {
						context.Request.Path }!</h1></body></html>");
			});
		}
	}

	public class CustomerController : Controller
	{
		[Route("customer/{id}")]
		public IActionResult GetCustomer(string id)
		{
			return new ObjectResult(new { customerId = id, customerName = $"Customer {id}" });
		}
	}
}
