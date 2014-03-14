using System.Linq;
using System.Web.Http;
using System.Web.Http.OData;

namespace WebApi
{
	[Authorize]
	public class CustomerController : ODataController
	{
		public CustomerController()
		{
		}

		[Queryable]
		public IQueryable<Customer> Get()
		{
			return CustomerRepository.Customers.AsQueryable();
		}
	}
}