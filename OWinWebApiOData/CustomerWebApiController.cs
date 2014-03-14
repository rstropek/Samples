using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.OData;

namespace WebApi
{
	[Authorize]
	public class CustomerWebApiController : ApiController
	{
		public CustomerWebApiController()
		{
		}

		// Gets
		[HttpGet]
		public Customer Get(int customerId)
		{
			return CustomerRepository.Customers
				.Where(c => c.ID == customerId)
				.SingleOrDefault();
		}

		// Gets
		[HttpGet]
		public IEnumerable<Customer> Get()
		{
			return CustomerRepository.Customers;
		}

		[HttpPost]
		public HttpResponseMessage Post(Customer c)
		{
			CustomerRepository.Customers.Add(c);
			return Request.CreateResponse(HttpStatusCode.Created);
		}
	}
}