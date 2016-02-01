using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

[assembly: OwinStartup(typeof(AdoNetProfiling.Common.Startup))]

namespace AdoNetProfiling.Common
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // Setup routes
            var config = new HttpConfiguration();

            // Removing XML formatter, we just want to support JSON
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            Startup.SetupWebApiRoutes(config);
            app.UseWebApi(config);
        }

        private static void SetupWebApiRoutes(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "webapi",
                routeTemplate: "api/{controller}",
                defaults: new { customerName = RouteParameter.Optional }
            );
        }
    }
}
