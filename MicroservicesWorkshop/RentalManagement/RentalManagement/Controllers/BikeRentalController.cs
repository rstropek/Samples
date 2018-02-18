using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RentalManagement.Model;
using RentalManagement.Services;

namespace RentalManagement.Controllers
{
    [Route("api/[controller]")]
    public class BikeRentalController : Controller
    {
        private IDataAccess dataAccess;
        private IRating rating;
        private IServiceBusSender serviceBusSender;

        public BikeRentalController(IDataAccess dataAccess = null, IRating rating = null, IServiceBusSender serviceBusSender = null)
        {
            this.dataAccess = dataAccess;
            this.rating = rating;
            this.serviceBusSender = serviceBusSender;
        }

        [HttpGet]
        [Route("dbName")]
        public async Task<string> Get() => 
            // Note that this is just a dummy method to show DB access is working
            await dataAccess.GetDbNameAsync();

        [HttpGet]
        [Route("{id}", Name = "getRental")]
        [ProducesResponseType(typeof(RentalEndResult), 200)]
        [ProducesResponseType(typeof(void), 500)]
        public IActionResult Get(string id)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [Route("end")]
        [ProducesResponseType(typeof(RentalEndResult), 201)]
        [ProducesResponseType(typeof(ErrorResult), 400)]
        public async Task<IActionResult> EndRental([FromBody]RentalEnd end)
        {
            #region Check prerequisites
            if (string.IsNullOrEmpty(end.CustomerID))
            {
                return BadRequest(new ErrorResult { Description = "Customer ID missing" });
            }

            if (string.IsNullOrEmpty(end.BikeID))
            {
                return BadRequest(new ErrorResult { Description = "Bike ID missing" });
            }
            #endregion

            // Remember end of rental
            var endOfRental = DateTime.UtcNow;

            // Simulate Data Access (e.g. read rental data from DB)
            await Task.Delay(250);

            // Simulate rental that started a while ago
            var beginOfRental = endOfRental.AddHours(Math.Round(new Random().NextDouble() * (-3), 1));

            // Use rating service to calculate costs
            var totalCosts = rating.CalculateTotalCosts(endOfRental - beginOfRental);

            // Build result object
            var result = new RentalEndResult
            {
                CustomerID = end.CustomerID,
                BikeID = end.BikeID,
                EndOfRental = endOfRental,
                BeginOfRental = beginOfRental,
                TotalCosts = totalCosts
            };

            // If service bus sender is available, send notification to service bus
            if (serviceBusSender != null)
            {
                await serviceBusSender.SendRentalEndResult(result);
            }

            return CreatedAtRoute("getRental", new { id = "BR999" }, result);
        }
    }
}
