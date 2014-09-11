using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using ODataFaq.DataModel;
using Owin;
using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Microsoft.Owin.Security;

[assembly: OwinStartup(typeof(ODataFaq.SelfHostService.Startup))]

namespace ODataFaq.SelfHostService
{
	class Program
	{
		static void Main(string[] args)
		{
			using (WebApp.Start<Startup>("http://localhost:12345"))
			{
				Console.WriteLine("Listening on port 12345. Press any key to quit.");
				Console.ReadLine();
			}
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			GlobalConfiguration.Configuration.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;

			var config = new HttpConfiguration();
			config.Routes.MapHttpRoute(
				name: "Customer",
				routeTemplate: "api/Customer/{id}",
				defaults: new { controller = "CustomerWebApi", id = RouteParameter.Optional }
			);
			config.Routes.MapHttpRoute(
				name: "CustomerByCountry",
				routeTemplate: "api/CustomerByCountry/{countryIsoCode}",
				defaults: new { controller = "CustomerByCountryWebApi" }
			);

			var builder = new ODataConventionModelBuilder();
			var customers = builder.EntitySet<Customer>("Customer");
			customers
				.EntityType
				.Collection
				.Function("OrderedBike")
				.ReturnsCollectionFromEntitySet<Customer>("Customer");
			config.MapODataServiceRoute(
				routeName: "odata",
				routePrefix: "odata",
				model: builder.GetEdmModel());

			// Removing XML formatter, we just want to support JSON
			config.Formatters.Remove(config.Formatters.XmlFormatter);

			app.UseWebApi(config);
		}
	}
}
