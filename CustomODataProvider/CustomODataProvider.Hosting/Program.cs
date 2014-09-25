using CustomODataProvider.Provider;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Web.Http;

namespace CustomODataProvider.Hosting
{
	class Program
	{
		static void Main(string[] args)
		{
			using (WebApp.Start<Startup>("http://localhost:5000")) 
			{ 
				Console.WriteLine( "Server ready... Press Enter to quit."); 
				Console.ReadLine(); 
			}
		}
	}

	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			var config = new HttpConfiguration();
			ODataConfiguration.RegisterOData(config);
			app.UseWebApi(config);
		}
	}
}
