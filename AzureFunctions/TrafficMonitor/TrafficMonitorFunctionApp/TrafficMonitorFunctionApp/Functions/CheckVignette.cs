using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using TrafficMonitor.Model;
using TrafficMonitor.Services;

namespace TrafficMonitorFunctionApp.Functions
{
    public class CheckVignette
    {
        private readonly IStorage storage;

        public CheckVignette(IStorage storage)
        {
            this.storage = storage;
        }

        /// <summary>
        /// Checks whether a car has a valid vignette
        /// </summary>
        [FunctionName("CheckVignette")]
        public async Task Run(
            [ServiceBusTrigger("plate-read", "check-vignette", Connection = "SECCTRL_RECEIVE_PLATE_READ")]PlateRead plate,
            [SignalR(HubName = nameof(RealTimeUIHub))] IAsyncCollector<SignalRMessage> signalRMessages,
            ILogger log)
        {
            log.LogInformation($"Start vignette check for {plate.LicensePlate}");
            var car = await storage.GetCarByLicensePlateAsync(plate.Nationality, plate.LicensePlate);

            if (car != null)
            {
                if (car.Vignettes == null || !car.Vignettes.Any(v => plate.ReadTimestamp >= v.From && plate.ReadTimestamp <= v.Until))
                {
                    log.LogInformation("Vignett violatation detected");
                    if (car.Violations == null)
                    {
                        car.Violations = new List<Violation>();
                    }

                    car.Violations.Add(new Violation(plate.ReadTimestamp, "Driving without vignette"));
                    await storage.UpdateCarAsync(car);

                    await signalRMessages.AddAsync(new SignalRMessage
                    {
                        Target = "vignetteViolation",
                        Arguments = new [] { new 
                            {
                                car.Nationality,
                                car.LicensePlate,
                                plate.CameraID
                            } 
                        }
                    });
                }
            }
            else
            {
                log.LogWarning("Car could not be found in DB");
                // TODO: Handle unknown car; out of scope for this example
            }
        }
    }
}
