using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace OwinPipeline
{
	using Microsoft.Owin;
	using AppFunc = Func<IDictionary<string, object>, Task>;

	public class Program
	{
		static void Main(string[] args)
		{
			using (WebApp.Start<Startup>("http://localhost:1337"))
			{
				Console.WriteLine("Listening ...");
				Console.ReadKey();
			}
		}
	}

	public class Startup
	{ 
		public void Configuration(IAppBuilder app)
		{
			//app.Run(async context => await context.Response.WriteAsync("Hello!"));

			app.Use(async (context, next) =>
			{
				await context.Response.WriteAsync("=== BEFORE ===");
				await next();
				await context.Response.WriteAsync("=== AFTER ===");
			});

			app.Map("/class", classApp => classApp.UseHelloWorld(
				new HelloWorldOptions() { Greeting = "Hello from Middleware Class" }));

			app.Map("/owin", owinApp =>
			{
				var middleware = new Func<AppFunc, AppFunc>(next =>
					async env =>
					{
						var bytes = Encoding.UTF8.GetBytes("Hello OWIN!");
						var headers = (IDictionary<string, string[]>)env["owin.ResponseHeaders"];
						headers["Content-Type"] = new[] { "text/html" };
						var response = (Stream)env["owin.ResponseBody"];
						await response.WriteAsync(bytes, 0, bytes.Length);

						await next(env);
					});
				owinApp.Use(middleware);
			});
		}
	}

	public class HelloWorldMiddleware
	{
		private readonly AppFunc next;

		private readonly HelloWorldOptions options;

		public HelloWorldMiddleware(AppFunc next, HelloWorldOptions options)
		{
			this.next = next;
			this.options = options;
		}

		public async Task Invoke(IDictionary<string, object> env)
		{
			var context = new OwinContext(env);
			context.Response.ContentType = "text/html";
			await context.Response.WriteAsync(this.options.Greeting);
			await this.next(env);
		}
	}

	public class HelloWorldOptions
	{
		public string Greeting { get; set; }
	}

	public static class HelloWorldMiddlewareExtension
	{
		public static void UseHelloWorld(this IAppBuilder app, HelloWorldOptions options)
		{
			app.Use<HelloWorldMiddleware>(options);
		}
	}
}