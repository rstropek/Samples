using Microsoft.Extensions.Logging.Debug;
using System;
using System.Threading.Tasks;
using TrafficMonitor.Functions;
using TrafficMonitor.Model;
using Xunit;

namespace TrafficMonitor.Tests
{
    public class CheckSectionControlTest
    {
        [Fact]
        public async Task RecognizeDuplicateEntry()
        {
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car { Nationality = "A", LicensePlate = "X", ActiveSection = new Enter() };
            var camera = new Camera { ID = "1", Start = new StartCamera() };
            await inMemoryStorage.CreateCarAsync(car);
            await inMemoryStorage.CreateCameraAsync(camera);

            await CheckSectionControl.Run(
                new PlateRead { CameraID = "1", Nationality = "A", LicensePlate = "X" }, 
                new DebugLogger(""), 
                inMemoryStorage);

            Assert.NotEmpty(car.Violations);
            Assert.Equal(1, inMemoryStorage.NumberOfCarUpdates);
        }

        [Fact]
        public async Task StoresActiveSessionOnEnter()
        {
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car { Nationality = "A", LicensePlate = "X" };
            var camera = new Camera { ID = "1", Start = new StartCamera() };
            await inMemoryStorage.CreateCarAsync(car);
            await inMemoryStorage.CreateCameraAsync(camera);

            await CheckSectionControl.Run(
                new PlateRead { CameraID = "1", Nationality = "A", LicensePlate = "X" },
                new DebugLogger(""),
                inMemoryStorage);

            Assert.Empty(car.Violations);
            Assert.NotNull(car.ActiveSection);
            Assert.Equal(1, inMemoryStorage.NumberOfCarUpdates);
        }

        [Fact]
        public async Task RecognizeExitWithoutEntry()
        {
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car { Nationality = "A", LicensePlate = "X" };
            var camera = new Camera { ID = "1", End = new EndCamera() };
            await inMemoryStorage.CreateCarAsync(car);
            await inMemoryStorage.CreateCameraAsync(camera);

            await CheckSectionControl.Run(
                new PlateRead { CameraID = "1", Nationality = "A", LicensePlate = "X" },
                new DebugLogger(""),
                inMemoryStorage);

            Assert.NotEmpty(car.Violations);
            Assert.Equal(1, inMemoryStorage.NumberOfCarUpdates);
        }

        [Fact]
        public async Task RecognizeCameraMismatch()
        {
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car { Nationality = "A", LicensePlate = "X", ActiveSection = new Enter { StartCameraID = "2" } };
            var camera = new Camera { ID = "1", End = new EndCamera() };
            await inMemoryStorage.CreateCarAsync(car);
            await inMemoryStorage.CreateCameraAsync(camera);

            await CheckSectionControl.Run(
                new PlateRead { CameraID = "1", Nationality = "A", LicensePlate = "X" },
                new DebugLogger(""),
                inMemoryStorage);

            Assert.NotEmpty(car.Violations);
            Assert.Equal(1, inMemoryStorage.NumberOfCarUpdates);
        }

        [Fact]
        public async Task RecognizeSpeedViolation()
        {
            var now = DateTime.UtcNow;
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car { Nationality = "A", LicensePlate = "X", ActiveSection = new Enter { StartCameraID = "1", Timestamp = now.AddHours(-0.75d).Ticks } };
            var camera = new Camera { ID = "2", End = new EndCamera { StartCameraID = "1", DistanceFromStart = 100000, MaximumAverageSpeed = 100 } };
            await inMemoryStorage.CreateCarAsync(car);
            await inMemoryStorage.CreateCameraAsync(camera);

            await CheckSectionControl.Run(
                new PlateRead { CameraID = "2", Nationality = "A", LicensePlate = "X", ReadTimestamp = now.Ticks },
                new DebugLogger(""),
                inMemoryStorage);

            Assert.NotEmpty(car.Violations);
            Assert.Null(car.ActiveSection);
            Assert.Equal(1, inMemoryStorage.NumberOfCarUpdates);
        }

        [Fact]
        public async Task RecognizeCorrectSpeedViolation()
        {
            var now = DateTime.UtcNow;
            var inMemoryStorage = new InMemoryStorage();
            var car = new Car { Nationality = "A", LicensePlate = "X", ActiveSection = new Enter { StartCameraID = "1", Timestamp = now.AddHours(-1d).Ticks } };
            var camera = new Camera { ID = "2", End = new EndCamera { StartCameraID = "1", DistanceFromStart = 100000, MaximumAverageSpeed = 100 } };
            await inMemoryStorage.CreateCarAsync(car);
            await inMemoryStorage.CreateCameraAsync(camera);

            await CheckSectionControl.Run(
                new PlateRead { CameraID = "2", Nationality = "A", LicensePlate = "X", ReadTimestamp = now.Ticks },
                new DebugLogger(""),
                inMemoryStorage);

            Assert.Empty(car.Violations);
            Assert.Null(car.ActiveSection);
            Assert.Equal(1, inMemoryStorage.NumberOfCarUpdates);
        }
    }
}
