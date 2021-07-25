using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
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

            // This is how you would return OData count property:
            // return Ok(new PageResult<Customer>(db.Customers, null, count: db.Customers.Count()));
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get(Guid key)
        {
            var customer = db.Customers.FirstOrDefault(p => p.CustomerId == key);
            if (customer == null)
            {
                return NotFound($"Not found customer with id = {key}");
            }

            return Ok(customer);
        }

        [EnableQuery]
        [HttpGet]
        [Route("OrderHeaders")]
        public IActionResult GetOrderHeaders(Guid key)
        {
            if (!db.Customers.Any(c => c.CustomerId == key))
            {
                return NotFound($"Not found customer with id = {key}");
            }

            return Ok(db.Orders.Where(o => o!.Customer!.CustomerId == key));
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult OrderedBike()
        {
            return Ok(from c in db.Customers
                      where c.OrderHeaders.Any(o => o!.OrderDetails!.Any(od => od!.Product!.CategoryCode == "BIKE"))
                      select c);
        }
    }
}
