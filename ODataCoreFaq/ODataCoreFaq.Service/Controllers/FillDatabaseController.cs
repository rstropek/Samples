using Microsoft.AspNetCore.Mvc;
using ODataCoreFaq.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ODataCoreFaq.Service.Controllers
{
    [Route("api/[Controller]")]
    public class FillDatabaseController : Controller
    {
        private OrderManagementContext db;

        public FillDatabaseController(OrderManagementContext context)
        {
            db = context;
        }

        [HttpGet]
        public async Task<IActionResult> FillDatabase()
        {
            await db.ClearAndFillWithDemoData();
            return Ok();
        }
    }
}
