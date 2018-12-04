using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using ODataCoreFaq.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCoreFaq.Service.Controllers
{
    public class OrdersController : ODataController
    {
        private OrderManagementContext db;

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
