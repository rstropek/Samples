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
	/// <summary>
	/// Trying to enhance performance by caching query result
	/// </summary>
	public class CachingSearchController : ApiController
	{
		private static DataTable customerCache = null;
		private static object cacheLockObject = new object();

		[HttpGet]
		public IHttpActionResult Get([FromUri]string customerName)
		{
			// Note double null-checking here. Reason: null-check is much faster than locking.
			if (customerCache == null)
			{
				lock (cacheLockObject)
				{
					if (customerCache == null)
					{
						using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString))
						{
							connection.Open();

							var addressTypePrimary = BasicSearchController.FetchMainOfficeAddressTypeID(connection);

							CachingSearchController.customerCache = new DataTable();
							BasicSearchController.QueryCustomers(connection, customerName, addressTypePrimary, false, customerCache);
						}
					}
				}
			}

            // This approach uses an ADO.NET DataView to query the cache.
            //var view = new DataView(CachingSearchController.customerCache);
            //view.RowFilter = "LastName LIKE '%" + customerName + "%' OR FirstName LIKE '%" + customerName + "%'";
            //return Ok(CachingSearchController.ConvertToJson(view.Cast<DataRowView>(), (row, colName) => row[colName]));

            // This approach replaces ADO.NET DataView with (stupid) LINQ.
            var rows = CachingSearchController.customerCache.Rows.Cast<DataRow>().ToArray();
            var tempResult = rows.Where(
                r => r["LastName"].ToString().ToUpper().Contains(customerName.ToUpper())
                    || r["FirstName"].ToString().ToUpper().Contains(customerName.ToUpper())).ToArray();
            return Ok(CachingSearchController.ConvertToJson(tempResult, (row, col) => row[col]));

            // And now with less stupid LINQ.
            //var customerNameUppercase = customerName.ToUpper();
            //var lastNameOrdinal = CachingSearchController.customerCache.Columns.IndexOf("UpperLastName");
            //var firstNameOrdinal = CachingSearchController.customerCache.Columns.IndexOf("UpperFirstName");
            //var tempResult = CachingSearchController.customerCache
            //	.Rows
            //	.Cast<DataRow>()
            //	.Where(r => r[lastNameOrdinal].ToString().Contains(customerNameUppercase)
            //			|| r[firstNameOrdinal].ToString().Contains(customerNameUppercase));
            //return Ok(CachingSearchController.ConvertToJson(tempResult, (row, col) => row[col]));
        }

		/// <summary>
		/// Helper function to convert a collection of data rows into JSON result
		/// </summary>
		/// <remarks>
		/// Much better implementation than in BasicSearch. Uses functional program to make algorithm general.
		/// </remarks>
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
