using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ODataCoreFaq.Service.Controllers
{
    [Route("api/[Controller]")]
    public class FillDatabaseController : Controller
    {
        private readonly OrderManagementContext db;

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
