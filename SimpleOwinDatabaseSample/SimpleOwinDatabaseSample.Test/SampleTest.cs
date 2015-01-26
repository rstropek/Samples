using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SimpleOwinDatabaseSample.Test
{
	[TestClass]
	public class SampleTest
	{
		public async Task RestoreTestDBAsync()
		{
			// In practice you would probably create a test database using DACPAC/BACPAC,
			// RESTORE, T-SQL script, etc. Here we create the test DB with two
			// sample tables just for demo purposes.

			var connectionString = ConfigurationManager.ConnectionStrings["SampleDBServer"].ConnectionString;
			var masterConnectionString = Regex.Replace(connectionString, "Database=[^;]+;", "Database=master;");
			using (var conn = new SqlConnection(masterConnectionString))
			{
				await conn.OpenAsync();
				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "SELECT MAX(database_id) FROM sys.databases WHERE name = 'OOP_ALM_Talk'";
					if (await cmd.ExecuteScalarAsync() == System.DBNull.Value)
					{
						cmd.CommandText = "CREATE DATABASE OOP_ALM_Talk";
						await cmd.ExecuteNonQueryAsync();

						cmd.CommandText = @"
							CREATE TABLE OOP_ALM_Talk.dbo.Dev ( ID INT PRIMARY KEY );
							CREATE TABLE OOP_ALM_Talk.dbo.Server ( ID INT PRIMARY KEY );";
						await cmd.ExecuteNonQueryAsync();
					}
				}
			}
		}

		[TestMethod]
		public void DummyTest()
		{
			// This is just a dummy test. If you want to check whether build fails
			// in case of a failing test, simply change this dummy test.
			Assert.IsTrue(1 == 1);
		}

		[TestMethod]
		public async Task DatabaseTest()
		{
			await RestoreTestDBAsync();

			// Note that this test really accesses a database. So we need a database
			// (localdb) on the build server.

			var pt = new PrivateType(typeof(SimpleOwinDatabaseSample.Startup));
			var tables = await (pt.InvokeStatic("GetTableNamesAsync") as Task<IEnumerable<string>>);
			Assert.IsNotNull(tables);
			Assert.IsTrue(tables.Count() > 0);
		}
	}
}
