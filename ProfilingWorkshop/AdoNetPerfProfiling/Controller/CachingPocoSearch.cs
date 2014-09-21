using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Http;
using Dapper;
using AdoNetPerfProfiling.DataAccess;

namespace AdoNetPerfProfiling.Controller
{
	/// <summary>
	/// Trying to enhance performance by caching query result
	/// </summary>
	public class CachingPocoSearchController : ApiController
	{
		internal class CustomerResult
		{
			public string LastName { get; set; }
			public string FirstName { get; set; }
			public string AddressLine1 { get; set; }
			public string AddressLine2 { get; set; }
			public string City { get; set; }
			public string CountryRegionName { get; set; }
			public string UpperLastName { get; set; }
			public string UpperFirstName { get; set; }
		}

		private static CustomerResult[] customerCache = null;
		private static object cacheLockObject = new object();

		[HttpGet]
		public IHttpActionResult Get([FromUri]string customerName)
		{
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

							CachingPocoSearchController.customerCache = connection.Query<CustomerResult>(
								new SelectBuilder() { IncludeNameFilter = false }.TransformText(),
								new { AddressTypeID = addressTypePrimary })
								.ToArray();
						}
					}
				}
			}

			var customerNameUppercase = customerName.ToUpper();
			var tempResult = CachingPocoSearchController.customerCache
				.Where(r => r.UpperFirstName.Contains(customerNameUppercase) || r.UpperLastName.Contains(customerNameUppercase));
			return Ok(tempResult);
		}
	}
}
