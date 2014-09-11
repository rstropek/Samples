using ODataFaq.DataModel;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ODataFaq.SelfHostService
{
	public class CustomerByCountryWebApiController : ApiController
	{
		[HttpGet]
		public IEnumerable<Customer> Get(string countryIsoCode)
		{
			using (var context = new OrderManagementContext())
			{
				return context.Customers
					.Where(c => c.CountryIsoCode == countryIsoCode)
					.ToArray();
			}
		}
	}
}
