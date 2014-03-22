namespace Snek.ToolsSample.Tests
{
	using Microsoft.QualityTools.Testing.Fakes;
	using Microsoft.SqlServer.Dac;
	using Microsoft.VisualStudio.TestTools.UnitTesting;
	using Snek.ToolsSample.Logic;
	using System;
	using System.Configuration;
	using System.Configuration.Fakes;
	using System.Data.SqlClient;
	using System.Diagnostics.CodeAnalysis;
	using System.Globalization;
	using System.IO;
	using System.Reflection;

	[TestClass]
	public class TimesheetDatabaseContextIntegrationTest
	{
		private static string testDatabaseName = null;

		private const string ConnectionStringTemplate = "Server=(localdb)\\v11.0;Database={0};Integrated Security=true";

		[ClassInitialize]
		public static void TestClassInitialize(TestContext context)
		{
			testDatabaseName = string.Format(CultureInfo.InvariantCulture, "TestRun_{0}", DateTime.Now.ToString("s", CultureInfo.InvariantCulture).Replace(':', '_'));

			var dacService = new DacServices(string.Format(CultureInfo.InvariantCulture, ConnectionStringTemplate, "master"));
			var dacPacLocation = Path.Combine(
				Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
				"Snek.ToolsSample.Database.dacpac");
			using (var stream = File.OpenRead(dacPacLocation))
			{
				using (var package = DacPackage.Load(stream))
				{
					dacService.Deploy(package, testDatabaseName);
				}
			}
		}

		[SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "Reviewed, is OK")]
		[ClassCleanup]
		public static void TestClassCleanup()
		{
			using (var conn = new SqlConnection(string.Format(CultureInfo.InvariantCulture, ConnectionStringTemplate, "master")))
			{
				conn.Open();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", testDatabaseName);
					cmd.ExecuteNonQuery();

					cmd.CommandText = string.Format(CultureInfo.InvariantCulture, "DROP DATABASE [{0}]", testDatabaseName);
					cmd.ExecuteNonQuery();
				}
			}
		}

		[TestMethod]
		[TestCategory("Integration Test")]
		public void TestGenerateDemoData()
		{
			using (ShimsContext.Create())
			{
				ShimConfigurationManager.ConnectionStringsGet =
					() => new ConnectionStringSettingsCollection()
						      {
							      new ConnectionStringSettings(
								      "TimesheetDatabase",
								      string.Format(CultureInfo.InvariantCulture, ConnectionStringTemplate, testDatabaseName))
						      };
				using (var db = new TimesheetDatabaseContext())
				{
					db.GenerateDemoData();
				}
			}
		}
	}
}
