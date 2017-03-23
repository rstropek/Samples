using Microsoft.OData.UriParser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;
using System.Web.OData.Routing;

namespace CustomODataProvider.Provider.Controller
{
	[ODataRoutePrefix("Customers")]
	public class CustomerController : ODataController
	{
		// Helpers for generating customer names
		private readonly char[] letters1 = "aeiou".ToArray();
		private readonly char[] letters2 = "bcdfgklmnpqrstvw".ToArray();
		private Random random = new Random();

		private const int pageSize = 100;

		[EnableQuery]
		[ODataRoute]
		public IHttpActionResult Get(ODataQueryOptions<Customer> options)
		{
			// Calculate number of results based on $top
			var numberOfResults = pageSize;
			if (options.Top != null)
			{
				numberOfResults = options.Top.Value;
			}

			// Analyze $filter
			string equalFilter = null;
			if (options.Filter != null)
			{
				// We only support a single "eq" filter
				var binaryOperator = options.Filter.FilterClause.Expression as BinaryOperatorNode;
				if (binaryOperator == null || binaryOperator.OperatorKind != BinaryOperatorKind.Equal)
				{
					return InternalServerError();
				}

				// One side has to be a reference to CustomerName property, the other side has to be a constant
				var propertyAccess = binaryOperator.Left as SingleValuePropertyAccessNode ?? binaryOperator.Right as SingleValuePropertyAccessNode;
				var constant = binaryOperator.Left as ConstantNode ?? binaryOperator.Right as ConstantNode;
				if (propertyAccess == null || propertyAccess.Property.Name != "CustomerName" || constant == null)
				{
					return InternalServerError();
				}

				// Save equal filter value
				equalFilter = constant.Value.ToString();

				// Return between 1 and 2 rows (CustomerName is not a primary key)
				numberOfResults = Math.Min(random.Next(1, 3), numberOfResults);
			}

			// Generate result
			var result = new List<Customer>();
			for (var i = 0; i < numberOfResults; i++)
			{
				result.Add(new Customer() { CustomerID = Guid.NewGuid(), CustomerName = equalFilter ?? GenerateCustomerName() });
			}

			return Ok(result.AsQueryable());
		}

		private string GenerateCustomerName()
		{
			var length = random.Next(5, 8);
			var result = new StringBuilder(length);
			for (var i = 0; i < length; i++)
			{
				var letter = (i % 2 == 0 ? letters1[random.Next(letters1.Length)] : letters2[random.Next(letters2.Length)]).ToString();
				result.Append(i == 0 ? letter.ToUpper() : letter);
			}

			return result.ToString();
		}
	}
}
