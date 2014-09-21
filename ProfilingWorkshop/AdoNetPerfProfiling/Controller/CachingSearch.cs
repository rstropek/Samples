using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace AdoNetPerfProfiling.Controller
{
	public class CachingSearchController : ApiController
	{
		private static DataTable cache = null;
		private static object cacheLockObject = new object();

		[HttpGet]
		public IHttpActionResult Get([FromUri]string customerName)
		{
			lock (cacheLockObject)
			{
				if (cache == null)
				{
					using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString))
					{
						connection.Open();

						var addressTypePrimary = BasicSearchController.FetchMainOfficeAddressTypeID(connection);

						cache = new DataTable();
						BasicSearchController.QueryCustomers(connection, customerName, addressTypePrimary, false, cache);
					}
				}
			}

			//var view = new DataView(cache);
			//view.RowFilter = "LastName LIKE '%" + customerName + "%' OR FirstName LIKE '%" + customerName + "%'";
			//return Ok(CachingSearchController.ConvertToJson(view.Cast<DataRowView>(), (row, colName) => row[colName]));

			//var rows = cache.Rows.Cast<DataRow>();
			//var tempResult = rows.Where(r => r["LastName"].ToString().ToUpper().Contains(customerName.ToUpper()) || r["FirstName"].ToString().ToUpper().Contains(customerName.ToUpper()));
			//return Ok(CachingSearchController.ConvertToJson(tempResult, (row, col) => row[col]));

			var upperCustomerName = customerName.ToUpper();
			var tempResult = cache.Rows
				.Cast<DataRow>()
				.Where(r => r["LastName"].ToString().ToUpper().Contains(upperCustomerName) || r["FirstName"].ToString().ToUpper().Contains(upperCustomerName));
			return Ok(CachingSearchController.ConvertToJson(tempResult, (row, col) => row[col]));

		}

		internal static JToken ConvertToJson<T>(IEnumerable<T> rows, Func<T, string, object> getColumn)
		{
			var jsonResult = new JArray();
			foreach (var row in rows)
			{
				var jsonRow = new JObject(
					new JProperty("LastName", getColumn(row, "LastName")),
					new JProperty("FirstName", getColumn(row, "FirstName")),
					new JProperty("AddressLine1", getColumn(row, "AddressLine1")),
					new JProperty("AddressLine2", getColumn(row, "AddressLine2")),
					new JProperty("City", getColumn(row, "City")),
					new JProperty("CountryRegionName", getColumn(row, "CountryRegionName")));
				jsonResult.Add(jsonRow);
			}

			return jsonResult;
		}
	}
}
