using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.WebJobs.Host;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TrafficMonitor.Model;

namespace TrafficMonitor.Services
{
    public class Storage : IStorage
    {
        public DocumentClient Client { get; private set; }

        private const string DbId = "TrafficManager";
        private const string CarsId = "Cars";
        private const string CamerasId = "Cameras";

        public Uri DbUri;
        public Uri CarsUri { get; set; }
        public Uri CamerasUri { get; set; }

        public Storage(Configuration configuration)
        {
            Client = new DocumentClient(new Uri(configuration.Host), configuration.AuthKey, new ConnectionPolicy
            {
                ConnectionMode = ConnectionMode.Direct,
                ConnectionProtocol = Protocol.Tcp
            });
            Client.OpenAsync().Wait();

            DbUri = UriFactory.CreateDatabaseUri(DbId);
            CarsUri = UriFactory.CreateDocumentCollectionUri(DbId, CarsId);
            CamerasUri = UriFactory.CreateDocumentCollectionUri(DbId, CamerasId);
        }

        public async Task DeleteDatabaseIfExists()
        {
            try
            {
                await Client.DeleteDatabaseAsync(DbUri);
            }
            catch (DocumentClientException)
            {
                // Ignore exceptions during DB deletion
            }
        }

        public async Task CreateDatabaseIfNotExistsAsync()
        {
            await Client.CreateDatabaseIfNotExistsAsync(new Database { Id = DbId });

            var carsCollection = new DocumentCollection();
            carsCollection.Id = CarsId;
            carsCollection.PartitionKey.Paths.Add("/lp");
            await Client.CreateDocumentCollectionIfNotExistsAsync(DbUri, carsCollection);

            var camerasCollection = new DocumentCollection();
            camerasCollection.Id = CamerasId;
            camerasCollection.PartitionKey.Paths.Add("/id");
            await Client.CreateDocumentCollectionIfNotExistsAsync(DbUri, camerasCollection);
        }

        public async Task CreateCarAsync(Car car) => await Client.CreateDocumentAsync(CarsUri, car);

        public async Task CreateCameraAsync(Camera camera) => await Client.CreateDocumentAsync(CamerasUri, camera);

        public async Task<Car> GetCarByLicensePlateAsync(string nationality, string licensePlate) =>
            await Client.CreateDocumentQuery<Car>(CarsUri)
                .Where(c => c.Nationality == nationality && c.LicensePlate == licensePlate)
                .FirstOrDefaultAsync();

        public async Task<Car> GetCarByIDAsync(string carID) =>
            await Client.CreateDocumentQuery<Car>(CarsUri, new FeedOptions { EnableCrossPartitionQuery = true })
                .Where(c => c.ID == carID)
                .FirstOrDefaultAsync();

        public async Task<Camera> GetCameraByIDAsync(string cameraID) =>
            await Client.CreateDocumentQuery<Camera>(CamerasUri)
                .Where(c => c.ID == cameraID)
                .FirstOrDefaultAsync();
        public async Task<int> GetNumberOfCars() =>
            (await Client.CreateDocumentQuery<int>(CarsUri, "SELECT VALUE COUNT(1) FROM c", new FeedOptions { EnableCrossPartitionQuery = true })
                .AsDocumentQuery().ExecuteNextAsync<int>()).First();

        public async Task UpdateCarAsync(Car car) => await Client.UpsertDocumentAsync(CarsUri, car);
    }

    public static class DocumentQueryExtensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IDocumentQuery<T> queryable)
        {
            var result = new List<T>();
            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<T>();
                result.AddRange(response);
            }

            return result;
        }
        public static async Task<T> FirstOrDefaultAsync<T>(this IDocumentQuery<T> queryable) where T : class
        {
            while (queryable.HasMoreResults)
            {
                var response = await queryable.ExecuteNextAsync<T>();
                if (response.Count > 0)
                {
                    return response.First();
                }
            }

            return default(T);
        }

        public static async Task<List<T>> ToListAsync<T>(this IQueryable<T> queryable) =>
            await queryable.AsDocumentQuery().ToListAsync();

        public static async Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> queryable) where T : class =>
            await queryable.AsDocumentQuery().FirstOrDefaultAsync();
    }
}
