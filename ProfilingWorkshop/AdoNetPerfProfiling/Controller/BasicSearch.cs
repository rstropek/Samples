using AdoNetPerfProfiling.DataAccess;
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
	/// Trivial implementation of for a customer search service
	/// </summary>
	public class BasicSearchController : ApiController
	{
		/// <summary>
		/// HTTP Getter
		/// </summary>
		/// <remarks>
		/// Note that this is a very trivial implementation with lots of problems. One of the most important ones is
		/// that the function is sync. We will have to make it async later.
		/// </remarks>
		[HttpGet]
		public IHttpActionResult Get([FromUri]string customerName)
		{
			try
			{
				// Open connection to database
				using (var connection = new SqlConnection(ConfigurationManager.ConnectionStrings["AdventureWorks"].ConnectionString))
				{
					connection.Open();

					var addressTypePrimary = BasicSearchController.FetchMainOfficeAddressTypeID(connection);

					var result = new DataTable();
					BasicSearchController.QueryCustomers(connection, customerName, addressTypePrimary, true, result);

					var jsonResult = BasicSearchController.ConvertToJson(result.Rows.Cast<DataRow>());
					return Ok(jsonResult);
				}
			}
			catch (Exception ex)
			{
				return InternalServerError(ex);
			}
		}

		/// <summary>
		/// Helper function to get address type ID of 'Main Office'
		/// </summary>
		internal static int FetchMainOfficeAddressTypeID(SqlConnection connection)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = "SELECT AddressTypeID FROM Person.AddressType WHERE Name = 'Main Office'";
				return (int)command.ExecuteScalar();
			}
		}

		/// <summary>
		/// Helper function to read all customers and put them into a data table
		/// </summary>
		internal static void QueryCustomers(SqlConnection connection, string customerName, int addressTypeID, bool includeNameFilter,  DataTable result)
		{
			using (var command = connection.CreateCommand())
			{
				command.CommandText = new SelectBuilder() { IncludeNameFilter = includeNameFilter }.TransformText();
				command.Parameters.AddWithValue("@customerName", customerName);
				command.Parameters.AddWithValue("@AddressTypeID", addressTypeID);
				using (var adapter = new SqlDataAdapter(command))
				{
					adapter.Fill(result);
				}
			}
		}

		/// <summary>
		/// Helper function to convert a collection of data rows into JSON result
		/// </summary>
		/// <remarks>
		/// Note that this implementation isn't very clever. It has a dependency on DataRow although it's core
		/// logic does only use a very tiny bit of DataRow's functionality. Bad design. We have to re-think this later.
		/// </remarks>
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
