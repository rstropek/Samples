using System.Threading.Tasks;
using TrafficMonitor.Model;

namespace TrafficMonitor.Services
{
    public interface IStorage
    {
        Task DeleteDatabaseIfExists();
        Task CreateDatabaseIfNotExistsAsync();
        Task CreateCarAsync(Car car);
        Task CreateCameraAsync(Camera camera);
        Task<Car> GetCarByLicensePlateAsync(string nationality, string licensePlate);
        Task<Car> GetCarByIDAsync(string carID);
        Task<Camera> GetCameraByIDAsync(string cameraID);
        Task UpdateCarAsync(Car car);
    }
}
