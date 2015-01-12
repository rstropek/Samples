using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WebHelloWorld.Startup))]

namespace WebHelloWorld
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.Run(context => context.Response.WriteAsync("<html><body><h1>Hello World!</h1></body></html>"));
		}
	}
}