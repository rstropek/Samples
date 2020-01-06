using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;

namespace ODataCoreFaq.Service.Controllers
{
    public class OrdersController : ODataController
    {
        private readonly OrderManagementContext db;

        public OrdersController(OrderManagementContext context)
        {
            db = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(db.Orders);
        }
    }
}
