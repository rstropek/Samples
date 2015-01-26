using Microsoft.Owin;
using Owin;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Net;
using System.Threading.Tasks;

[assembly: OwinStartup(typeof(SimpleOwinDatabaseSample.Startup))]

namespace SimpleOwinDatabaseSample
{
	public class Startup
	{
		#region HTML Fragments
		private const string HtmlHeader = @"<!doctype html>
<html>
<body>
	<h1>Simple OWIN Database Sample</h1>
";
		private const string ServerInfoHeader = @"	<h2>Server</h2>
	<p>
";
		private const string ServerInfoFooter = @"	</p>
";
		private const string TableListHeader = @"	<h2>List of Tables</h2>
	<ul>
";
		private const string TableListFooter = @"	</ul>
";
		private const string HtmlFooter = @"</body>
</html>";
		#endregion

		public void Configuration(IAppBuilder app)
		{
			app.Run(async context =>
			{
				if (context.Request.Method != "GET")
				{
					context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
				}

				await context.Response.WriteAsync(HtmlHeader);

				await context.Response.WriteAsync(ServerInfoHeader);
				await context.Response.WriteAsync(string.Format("	Local IP: {0}<br/>\n", context.Request.LocalIpAddress));
				await context.Response.WriteAsync(string.Format("	Local Port: {0}<br/>\n", context.Request.LocalPort));
				await context.Response.WriteAsync(string.Format("	Remote IP: {0}<br/>\n", context.Request.RemoteIpAddress));
				await context.Response.WriteAsync(string.Format("	Remote Port: {0}<br/>\n", context.Request.RemotePort));
				await context.Response.WriteAsync(ServerInfoFooter);
	
				await context.Response.WriteAsync(TableListHeader);

				foreach (var tableName in await GetTableNamesAsync())
				{
					await context.Response.WriteAsync(string.Format("		<li>{0}</li>\n", tableName));
				}

				await context.Response.WriteAsync(TableListFooter);
				await context.Response.WriteAsync(HtmlFooter);
			});
		}

		private static async Task<IEnumerable<string>> GetTableNamesAsync()
		{
			var result = new List<string>();
			var connectionString = ConfigurationManager.ConnectionStrings["SampleDBServer"].ConnectionString;
			using (var conn = new SqlConnection(connectionString))
			{
				await conn.OpenAsync();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES";
					using (var reader = await cmd.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							result.Add(reader.GetString(0));
						}
					}
				}
			}

			return result;
		}
	}
}