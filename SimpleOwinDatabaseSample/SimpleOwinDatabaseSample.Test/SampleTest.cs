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
			// Note that this test really accesses a database. So we need a database
			// (localdb) on the build server.

			var pt = new PrivateType(typeof(SimpleOwinDatabaseSample.Startup));
			var tables = await (pt.InvokeStatic("GetTableNamesAsync") as Task<IEnumerable<string>>);
			Assert.IsNotNull(tables);
			Assert.IsTrue(tables.Count() > 0);
		}
	}
}
