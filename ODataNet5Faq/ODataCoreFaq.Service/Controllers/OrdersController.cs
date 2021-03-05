using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

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
