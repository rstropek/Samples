using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;

namespace OwinSimpleServer
{
	public class Program
	{
		static void Main(string[] args)
		{
			using (WebApp.Start<Program>("http://localhost:1337"))
			{
				Console.WriteLine("Listening ...");
				Console.ReadKey();
			}
		}

		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();
			config.MapHttpAttributeRoutes();
			app.UseWebApi(config);

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

	public class CustomerController : ApiController
	{
		[Route("customer/{id}")]
		public IHttpActionResult GetCustomer(string id)
		{
			return this.Ok(new { customerId = id, customerName = $"Customer {id}" });
		}
	}
}
