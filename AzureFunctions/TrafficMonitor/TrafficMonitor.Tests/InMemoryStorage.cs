using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficMonitor.Model;
using TrafficMonitor.Services;

namespace TrafficMonitor.Tests
{
    /// <summary>
    /// In-memory mock implementation of storage for unit testing
    /// </summary>
    public class InMemoryStorage : IStorage
    {
        private static readonly Task EmptyTask = Task.FromResult(0);
        private List<Camera> Cameras { get; set; } = new List<Camera>();
        private List<Car> Cars { get; set; } = new List<Car>();

        public int NumberOfCarUpdates { get; private set; }

        public Task CreateCameraAsync(Camera camera)
        {
            Cameras.Add(camera);
            return EmptyTask;
        }

        public Task CreateCarAsync(Car car)
        {
            Cars.Add(car);
            return EmptyTask;
        }

        public Task CreateDatabaseIfNotExistsAsync() => EmptyTask;

        public Task DeleteDatabaseIfExists()
        {
            Cameras.Clear();
            Cars.Clear();
            return EmptyTask;
        }

        public Task<Camera> GetCameraByIDAsync(string cameraID) =>
            Task.FromResult(Cameras.FirstOrDefault(c => c.ID == cameraID));

        public Task<Car> GetCarByLicensePlateAsync(string nationality, string licensePlate) =>
            Task.FromResult(Cars.FirstOrDefault(c => c.Nationality == nationality && c.LicensePlate == licensePlate));
        public Task<Car> GetCarByIDAsync(string carID) =>
            Task.FromResult(Cars.FirstOrDefault(c => c.ID == carID));

        public Task UpdateCarAsync(Car car)
        {
            NumberOfCarUpdates++;
            return EmptyTask;
        }
    }
}
