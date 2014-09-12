using ODataFaq.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using System.Security.Claims;
using System.Threading;

namespace ODataFaq.SelfHostService
{
	[RoutePrefix("Customer")]
	public class CustomerWebApiController : ApiController
	{
		[HttpGet]
		[Route("")]
		public IEnumerable<Customer> Get()
		{
			using (var context = new OrderManagementContext())
			{
				return context.Customers.ToArray();
			}
		}

		[HttpGet]
		[Route("{id:Guid}")]
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
