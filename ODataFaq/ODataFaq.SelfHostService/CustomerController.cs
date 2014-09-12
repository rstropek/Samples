using ODataFaq.DataModel;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Routing;

namespace ODataFaq.SelfHostService
{
	[Authorize]
	[ODataRoutePrefix("Customer")]
	public class CustomerController : ODataController
	{
		private OrderManagementContext context = new OrderManagementContext();

		[EnableQuery]
		public /*IQueryable<Customer>*/ IHttpActionResult Get()
		{
			if (!string.IsNullOrWhiteSpace(((ClaimsPrincipal)Thread.CurrentPrincipal).Claims.FirstOrDefault(c => c.Type == "IsAdmin").Value))
			{
				return Ok(context.Customers);
			}

			return Unauthorized();
		}

		[EnableQuery]
		[ODataRoute("Default.OrderedBike")]
		[HttpGet]
		public IQueryable<Customer> OrderedBike()
		{
			return from c in this.context.Customers
				   where c.Orders.Count(o => o.OrderDetails.Count(od => od.Product.CategoryCode == "BIKE") > 0) > 0
				   select c;
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				this.context.Dispose();
				GC.SuppressFinalize(this);
			}
		}
	}
}
