using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace WebApiDemo.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class FillDatabaseController : ControllerBase
    {
        private readonly OrderManagementContext db;

        public FillDatabaseController(OrderManagementContext context)
        {
            db = context;
        }

        /// <summary>
        /// Fill database with demo content
        /// </summary>
        [HttpGet]
        public async Task FillDatabase()
        {
            await db.ClearAndFillWithDemoData();
        }
    }
}
