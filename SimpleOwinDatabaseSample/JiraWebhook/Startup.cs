using Microsoft.Owin;
using Owin;
using System.Web.Http;

[assembly: OwinStartup(typeof(JiraWebhook.Startup))]

namespace JiraWebhook
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();
			config.MapHttpAttributeRoutes();
			app.UseWebApi(config);
		}
	}
}