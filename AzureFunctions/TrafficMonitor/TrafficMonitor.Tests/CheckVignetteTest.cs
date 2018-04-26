using Microsoft.Extensions.Logging.Debug;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TrafficMonitor.Functions;
using TrafficMonitor.Model;
using Xunit;

namespace TrafficMonitor.Tests
{
    public class CheckVignetteTest
    {
        [Fact]
        public async Task RecognizeMissingVignette()
        {
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car { LicensePlate = "X" };
            await inMemoryStorage.CreateCarAsync(car);

            await CheckVignette.Run(new PlateRead { LicensePlate = "X" }, new DebugLogger(""), inMemoryStorage);

            Assert.NotEmpty(car.Violations);
        }

        [Fact]
        public async Task DetectValidVignette()
        {
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car
            {
                LicensePlate = "X",
                Vignettes = new List<Vignette>
                {
                    new Vignette
                    {
                        From = DateTime.UtcNow.AddMinutes(-5).Ticks,
                        Until = DateTime.UtcNow.AddMinutes(5).Ticks
                    }
                }
            };
            await inMemoryStorage.CreateCarAsync(car);

            await CheckVignette.Run(
                new PlateRead{ LicensePlate = "X", ReadTimestamp = DateTime.UtcNow.Ticks }, 
                new DebugLogger(""), 
                inMemoryStorage);

            Assert.Empty(car.Violations);
        }

        [Fact]
        public async Task CheckGracefulFailureForUnknownCars()
        {
            var inMemoryStorage = new InMemoryStorage();
            await CheckVignette.Run(new PlateRead { LicensePlate = "X" }, new DebugLogger(""), inMemoryStorage);
        }
    }
}
