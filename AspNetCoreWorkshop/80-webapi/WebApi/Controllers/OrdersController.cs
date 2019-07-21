using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DataModel;
using WebApiDemo;

namespace WebApiDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly OrderManagementContext _context;

        public OrdersController(OrderManagementContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderHeader>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderHeader>> GetOrderHeader(Guid id)
        {
            var orderHeader = await _context.Orders.FindAsync(id);

            if (orderHeader == null)
            {
                return NotFound();
            }

            return orderHeader;
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrderHeader(Guid id, OrderHeader orderHeader)
        {
            if (id != orderHeader.OrderId)
            {
                return BadRequest();
            }

            _context.Entry(orderHeader).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderHeaderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<OrderHeader>> PostOrderHeader(OrderHeader orderHeader)
        {
            _context.Orders.Add(orderHeader);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrderHeader", new { id = orderHeader.OrderId }, orderHeader);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<OrderHeader>> DeleteOrderHeader(Guid id)
        {
            var orderHeader = await _context.Orders.FindAsync(id);
            if (orderHeader == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(orderHeader);
            await _context.SaveChangesAsync();

            return orderHeader;
        }

        private bool OrderHeaderExists(Guid id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
