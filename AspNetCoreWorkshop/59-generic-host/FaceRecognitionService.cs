using FaceRecognitionDotNet;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GenericHost
{
    class FaceRecognitionService : IHostedService
    {
        private readonly ILogger logger;

        public FaceRecognitionService(ILogger<FaceRecognitionService> logger)
        {
            this.logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Starting face recognition service");

            var faceRecognition = FaceRecognition.Create(@"C:\Code\GitHub\Samples\AspNetCoreWorkshop\59-generic-host\Images");
            using (var image = FaceRecognition.LoadImageFile("city-community-crossing.jpg"))
            {
                var faces = faceRecognition.FaceLocations(image, 0, Model.Cnn);
                foreach(var face in faces)
                {
                    Console.WriteLine($"{face.Top},{face.Right},{face.Bottom},{face.Left}");
                }
            }


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Stopping face recognition service");
            return Task.CompletedTask;
        }
    }
}
