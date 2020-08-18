using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TrafficMonitor.Services
{
    /// <summary>
    /// Represents the result of a license plate recognition
    /// </summary>
    public class RecognitionResult
    {
        public string Plate { get; set; }

        public double Confidence { get; set; }

        public string Region { get; set; }

        [JsonProperty(PropertyName = "region_confidence")]
        public double RegionConfidence { get; set; }
    }

    public interface ILicensePlateRecognizer
    {
        Task<RecognitionResult> RecognizeAsync(byte[] image, Configuration configuration);
    }

    /// <summary>
    /// Implements a license plate recognizer that uses the OpenALPR Cloud API
    /// </summary>
    public class OpenAlprRecognizer : ILicensePlateRecognizer
    {
        private static readonly HttpClient OpenAlprClient = new HttpClient()
        {
            BaseAddress = new Uri("https://api.openalpr.com/v2/")
        };

        private class AlprResult
        {
            public List<RecognitionResult> Results { get; set; }
        }

        public async Task<RecognitionResult> RecognizeAsync(byte[] image, Configuration configuration)
        {
            var fileBase64 = Convert.ToBase64String(image);

            // Send image to OpenALPR for license plate recognition
            using var response = await OpenAlprClient.PostAsJsonAsync(
                $"recognize_bytes?secret_key={configuration.OpenAlprKey}&country=eu&topn=1",
                fileBase64);
            response.EnsureSuccessStatusCode();

            // Deserialize result
            var resultString = Encoding.UTF8.GetString(await response.Content.ReadAsByteArrayAsync());
            var result = JsonConvert.DeserializeObject<AlprResult>(resultString);
            if (result == null || result.Results == null || result.Results.Count == 0)
            {
                return null;
            }

            return result.Results[0];
        }
    }

    /// <summary>
    /// Implements a mock object for a license plate recognizer
    /// </summary>
    public class MockupRecognizer : ILicensePlateRecognizer
    {
        private static int? NumberOfCars = null;

        public async Task<RecognitionResult> RecognizeAsync(byte[] image, Configuration configuration)
        {
            await Task.Delay(1000);

            var storage = new Storage(configuration);
            if (!NumberOfCars.HasValue)
            {
                NumberOfCars = await storage.GetNumberOfCars();
            }

            var randomCar = await storage.GetCarByIDAsync(new Random().Next(NumberOfCars.Value).ToString());

            return new RecognitionResult
            {
                Plate = randomCar.LicensePlate,
                Confidence = 95d,
                Region = randomCar.Nationality,
                RegionConfidence = 95d
            };
        }
    }
}
