using ODataFaq.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace ODataFaq.SelfHostService
{
	public class CustomerWebApiController : ApiController
	{
		[HttpGet]
		public IEnumerable<Customer> Get()
		{
			using (var context = new OrderManagementContext())
			{
				return context.Customers.ToArray();
			}
		}

		[HttpGet]
		public Customer Get(Guid id)
		{
			using (var context = new OrderManagementContext())
			{
				return context.Customers
					.SingleOrDefault(c => c.CustomerId == id);
			}
		}
	}
}
