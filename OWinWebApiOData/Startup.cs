using Owin;
using System.Web.Http;
using System.Web.Http.OData.Builder;
using Microsoft.Owin.StaticFiles;
using Microsoft.Owin;
using Microsoft.Owin.FileSystems;

namespace WebApi
{
    // Note: By default all requests go through this OWIN pipeline. Alternatively you can turn this off by adding an appSetting owin:AutomaticAppStartup with value “false”. 
    // With this turned off you can still have OWIN apps listening on specific routes by adding routes in global.asax file using MapOwinPath or MapOwinRoute extensions on RouteTable.Routes
    public class Startup
    {
        // Invoked once at startup to configure your application.
        public void Configuration(IAppBuilder builder)
        {
			builder.Use((context, next) => { context.Response.Headers.Add("MyHeader", new [] { "abc" }); return next(); });

			builder.UseStaticFiles("/Client");
			//builder.UseFileServer(new FileServerOptions()
			//{
			//	RequestPath = new PathString("/Client"),
			//	FileSystem = new PhysicalFileSystem(".\\Client")
			//});

			builder.Use<BasicAuthentication>();

			HttpConfiguration config = new HttpConfiguration();
			config.Routes.MapHttpRoute("Default", "api/Customer/{customerID}", new { controller="CustomerWebApi", customerID = RouteParameter.Optional });

			var oDataBuilder = new ODataConventionModelBuilder();
			oDataBuilder.EntitySet<Customer>("Customer");
			config.Routes.MapODataRoute("odata", "odata", oDataBuilder.GetEdmModel());

			// Use this if you want XML instead of JSON
			//config.Formatters.XmlFormatter.UseXmlSerializer = true;
			//config.Formatters.Remove(config.Formatters.JsonFormatter);

            config.Formatters.JsonFormatter.UseDataContractJsonSerializer = true;
			//config.Formatters.Remove(config.Formatters.XmlFormatter);

            builder.UseWebApi(config);
		}
    }
}