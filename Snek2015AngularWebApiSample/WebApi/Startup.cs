using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Dependencies;

[assembly: OwinStartup(typeof(WebApi.Startup))]

namespace WebApi
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			app.UseCors(CorsOptions.AllowAll);

			var config = new HttpConfiguration();
			config.MapHttpAttributeRoutes();
			config.Formatters.JsonFormatter.SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/html"));
			app.UseWebApi(config);
		}
	}
}
