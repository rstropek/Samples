using Microsoft.OData.Edm;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using System.Web.OData.Routing.Conventions;

namespace CustomODataProvider.Provider
{
	public static class ODataConfiguration
	{
		public static void RegisterOData(HttpConfiguration config)
		{
			config.Formatters.Clear();
			config.Formatters.Add(new JsonMediaTypeFormatter());

			var routeConventions = ODataRoutingConventions.CreateDefault();
			config.MapODataServiceRoute("odata", "odata", GetModel());
		}

		private static IEdmModel GetModel()
		{
			var modelBuilder = new ODataConventionModelBuilder();
			modelBuilder.EntitySet<Customer>("Customers");
			return modelBuilder.GetEdmModel();
		}
	}
}
