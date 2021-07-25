using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ODataCoreFaq.Service.Controllers
{
    public class OrderDetailsController : ODataController
    {
        private readonly OrderManagementContext db;

        public OrderDetailsController(OrderManagementContext context)
        {
            db = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(db.OrderDetails);
        }


        [EnableQuery]
        [HttpGet]
        public IActionResult Get(Guid key)
        {
            var order = db.OrderDetails.FirstOrDefault(o => o.OrderDetailId == key);
            if (order == null)
            {
                return NotFound($"Not found order detail with id = {key}");
            }

            return Ok(order);
        }

        [HttpGet("Product")]
        public async Task<IActionResult> GetProduct(Guid key)
        {
            var od = await db.OrderDetails.Include(od => od.Product)
                .FirstOrDefaultAsync(o => o!.OrderDetailId == key);
            if (od == null)
            {
                return NotFound($"Not found order detail with id = {key}");
            }

            return Ok(od?.Product);
        }
    }
}
