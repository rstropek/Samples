using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;
using ODataCoreFaq.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCoreFaq.Service.Controllers
{
    public record OrderStatistic(Guid OrderId, DateTimeOffset OrderDate, Guid CustomerId,
        string CompanyName, string CountryIsoCode, int TotalAmount);

    public class OrderHeadersController : ODataController
    {
        private readonly OrderManagementContext db;

        public OrderHeadersController(OrderManagementContext context)
        {
            db = context;
        }

        [EnableQuery]
        public IActionResult Get()
        {
            return Ok(db.Orders.Include(o => o.OrderDetails).Include(o => o.Customer));
        }


        [EnableQuery]
        [HttpGet]
        public IActionResult Get(Guid key)
        {
            var order = db.Orders.FirstOrDefault(o => o.OrderId == key);
            if (order == null)
            {
                return NotFound($"Not found order with id = {key}");
            }

            return Ok(order);
        }

        [EnableQuery]
        [HttpGet("OrderDetails")]
        public IActionResult GetOrderDetails(Guid key)
        {
            if (!db.Orders.Any(o => o.OrderId == key))
            {
                return NotFound($"Not found order with id = {key}");
            }

            return Ok(db.OrderDetails.Where(o => o!.Order!.OrderId == key));
        }


        [HttpGet("Customer")]
        public async Task<IActionResult> GetCustomer(Guid key)
        {
            var o = await db.Orders.Include(o => o.Customer)
                .FirstOrDefaultAsync(o => o!.OrderId == key);
            if (o == null)
            {
                return NotFound($"Not found order with id = {key}");
            }

            return Ok(o?.Customer);
        }

        [HttpGet]
        public async Task<IEnumerable<OrderStatistic>> CalculateStatistics()
        {
            return await db.Orders
                .Select(o => new OrderStatistic(
                    o.OrderId,
                    o.OrderDate,
                    o.Customer!.CustomerId,
                    o.Customer.CompanyName,
                    o.Customer.CountryIsoCode,
                    o.OrderDetails!.Sum(od => od.Amount)
                )).ToArrayAsync();
        }

    }
}
