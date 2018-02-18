using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentalManagement.Services;

namespace RentalManagement.Controllers
{
    [Route("api/[controller]")]
    public class BikeRentalController : Controller
    {
        private IDataAccess dataAccess;

        public BikeRentalController(IDataAccess dataAccess)
        {
            this.dataAccess = dataAccess;
        }

        [HttpGet]
        public async Task<string> Get() => await dataAccess.GetDbNameAsync();
    }
}
