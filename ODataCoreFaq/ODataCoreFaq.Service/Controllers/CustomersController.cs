using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ODataCoreFaq.Service.Controllers
{
    public class CustomersController : ODataController
    {
        private readonly OrderManagementContext db;

        public CustomersController(OrderManagementContext context)
        {
            db = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(db.Customers);
        }


        [EnableQuery]
        [HttpGet]
        public IActionResult OrderedBike()
        {
            return Ok(from c in db.Customers
                      where c.Orders.Count(o => o.OrderDetails.Count(od => od.Product.CategoryCode == "BIKE") > 0) > 0
                      select c);
        }
    }
}
