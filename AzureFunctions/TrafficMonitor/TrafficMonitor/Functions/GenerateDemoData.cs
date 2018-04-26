using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using System.Net;
using TrafficMonitor.Model;
using TrafficMonitor.Services;
using Microsoft.Extensions.Logging;

namespace TrafficMonitor.Functions
{
    public static class GenerateDemoData
    {
        #region Helper variables
        private static readonly List<Vignette> DemoVignettes = new List<Vignette> {
            new Vignette { From = new DateTime(2017, 1, 1).Ticks, Until = new DateTime(2017, 12, 31).Ticks},
            new Vignette { From = new DateTime(2018, 1, 1).Ticks, Until = new DateTime(2018, 12, 31).Ticks}
        };

        private static readonly IActionResult CreatedResult = new StatusCodeResult((int)HttpStatusCode.Created);
        private static readonly IActionResult ErrorResult = new StatusCodeResult((int)HttpStatusCode.InternalServerError);
        #endregion

        /// <summary>
        /// Generates demo data
        /// </summary>
        /// <remarks>
        /// This function generates demo data in Azure CosmosDB. In practice, such a service could exist for
        /// e.g. importing car data from an external system. The function demonstrates how to use Azure Functions's 
        /// HTTP trigger. Note how we use dependency injection to get application services. In a unit test,
        /// mock objects could be provided for these services.
        /// </remarks>
        [FunctionName("GenerateDemoData")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Admin, "post", Route = "generate-demo-data")]HttpRequest req,
            ILogger log,
            [Inject(typeof(Configuration))]Configuration configuration,
            [Inject(typeof(IStorage))]IStorage storage,
            [Inject(typeof(LicensePlateGenerator))]LicensePlateGenerator licensePlateGenerator,
            [Inject(typeof(VehicleCategoryGenerator))]VehicleCategoryGenerator vehicleCategoryGenerator)
        {
            log.LogInformation("Starting to re-generate demo data");
            try
            {
                log.LogInformation("Deleting existing database if exists");
                await storage.DeleteDatabaseIfExists();

                log.LogInformation("Creating demo database");
                await storage.CreateDatabaseIfNotExistsAsync();

                log.LogInformation("Filling demo database");
                foreach (var car in GetCarsToImport(configuration.NumberOfAustrianCars, configuration.NumberOfGermanCars, licensePlateGenerator, vehicleCategoryGenerator))
                {
                    await storage.CreateCarAsync(car);
                }

                foreach (var camera in GetCamerasToImport(configuration.NumberOfAustrianSectionControls, configuration.NumberOfGermanSectionControls))
                {
                    await storage.CreateCameraAsync(camera);
                }

                log.LogInformation("Generating demo data finished");
                return CreatedResult;
            }
            catch (Exception ex)
            {
                log.LogError(ex.ToString());
                return ErrorResult;
            }
        }

        #region Helper functions for generating demo data
        private static IEnumerable<Car> GetCarsToImport(
            int numberOfAustrianCars,
            int numberOfGermanCars,
            LicensePlateGenerator licensePlateGenerator,
            VehicleCategoryGenerator vehicleCategoryGenerator)
        {
            // Generate some Austrian cars
            var i = 0;
            for (; i < numberOfAustrianCars; i++)
            {
                yield return new Car
                {
                    ID = i.ToString(),
                    Nationality = "eu-at",
                    VehicleCategory = vehicleCategoryGenerator.GetRandomVehicleClass(),
                    LicensePlate = licensePlateGenerator.GetRandomAustrianLicensePlate(),
                    Vignettes = (i % 10 != 0) ? DemoVignettes : null
                };
            }

            // Generate some German cars
            for (; i < numberOfAustrianCars + numberOfGermanCars; i++)
            {
                yield return new Car
                {
                    ID = i.ToString(),
                    Nationality = "eu-de",
                    VehicleCategory = vehicleCategoryGenerator.GetRandomVehicleClass(),
                    LicensePlate = licensePlateGenerator.GetRandomGermanLicensePlate(),
                    Vignettes = (i % 10 != 0) ? DemoVignettes : null
                };
            }

            // Generate a special car for which we have a demo image
            yield return new Car
            {
                ID = i.ToString(),
                Nationality = "eu-de",
                VehicleCategory = "M",
                LicensePlate = "BIES770"
            };
        }

        private static IEnumerable<Camera> GetCamerasToImport(int numberOfAustrianSectionControls, int numberOfGermanSectionControls)
        {
            // Generate some Austrian section controls
            var i = 0;
            for (; i < numberOfAustrianSectionControls; i += 2)
            {
                yield return new Camera
                {
                    ID = i.ToString(),
                    Location = "eu-at",
                    Start = new StartCamera { EndCameraID = (i + 1).ToString() }
                };
                yield return new Camera
                {
                    ID = (i + 1).ToString(),
                    Location = "eu-at",
                    End = new EndCamera { StartCameraID = i.ToString(), DistanceFromStart = 9500, MaximumAverageSpeed = 100 }
                };
            }

            // Generate some German section controls
            for (; i < numberOfAustrianSectionControls + numberOfGermanSectionControls; i += 2)
            {
                yield return new Camera
                {
                    ID = i.ToString(),
                    Location = "eu-de",
                    Start = new StartCamera { EndCameraID = (i + 1).ToString() }
                };
                yield return new Camera
                {
                    ID = (i + 1).ToString(),
                    Location = "eu-de",
                    End = new EndCamera { StartCameraID = i.ToString(), DistanceFromStart = 11500, MaximumAverageSpeed = 80 }
                };
            }
        }
        #endregion
    }
}
