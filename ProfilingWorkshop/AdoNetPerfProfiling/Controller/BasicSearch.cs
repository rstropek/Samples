using AdoNetPerfProfiling.DataAccess;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;

namespace AdoNetPerfProfiling.Controller
{
	/// <summary>
	/// Trivial implementation of for a customer search service
	/// </summary>
	public class BasicSearchController : ApiController
	{
		[HttpGet]
		public IHttpActionResult Get([FromUri]string customerName)
		{
			using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString))
			{
				connection.Open();

				var addressTypePrimary = BasicSearchController.FetchMainOfficeAddressTypeID(connection);
				
				var result = new DataTable();
				BasicSearchController.QueryCustomers(connection, customerName, addressTypePrimary, true, result);

				return Ok(CachingSearchController.ConvertToJson(result.Rows.Cast<DataRow>(), (row, colName) => row[colName]));
			}
		}

		internal static int FetchMainOfficeAddressTypeID(SqlConnection connection)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = "SELECT AddressTypeID FROM Person.AddressType WHERE Name = 'Main Office'";
				return (int)command.ExecuteScalar();
			}
		}

		internal static void QueryCustomers(SqlConnection connection, string customerName, int addressTypeID, bool includeNameFilter,  DataTable result)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandTimeout = 600;
				command.CommandText = new SelectBuilder() { IncludeNameFilter = includeNameFilter }.TransformText();
				command.Parameters.AddWithValue("@customerName", customerName);
				command.Parameters.AddWithValue("@AddressTypeID", addressTypeID);
				using (var adapter = new SqlDataAdapter(command))
				{
					adapter.Fill(result);
				}
			}
		}

		private static JToken ConvertToJson(IEnumerable<DataRow> rows)
		{
			var jsonResult = new JArray();
			foreach (var row in rows)
			{
				var jsonRow = new JObject(
					new JProperty("LastName", row["LastName"]),
					new JProperty("FirstName", row["FirstName"]),
					new JProperty("AddressLine1", row["AddressLine1"]),
					new JProperty("AddressLine2", row["AddressLine2"]),
					new JProperty("City", row["City"]),
					new JProperty("CountryRegionName", row["CountryRegionName"]));
				jsonResult.Add(jsonRow);
			}

			return jsonResult;
		}
	}
}
