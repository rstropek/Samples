using Microsoft.WindowsAzure.Storage;
using System;
using System.Threading;

namespace TrafficMonitor.Generator
{
    class Program
    {

        static void Main(string[] args)
        {
            #region Connection string
            const string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=devone18;AccountKey=xCrjJJ4uyL/wUEqIydd3q5QFi1FJoldqyEZKLTOOXO2HDTdrq5D31MubB7jCaB2zthRTtyQpZBtHwMagBdb5cA==;EndpointSuffix=core.windows.net";
            #endregion

            const double messagesPerSecond = 0.2d;

            CloudStorageAccount.TryParse(storageConnectionString, out var storageAccount);

            var cloudBlobClient = storageAccount.CreateCloudBlobClient();
            var cloudBlobContainer = cloudBlobClient.GetContainerReference("car-images");

            while (true)
            {
                Thread.Sleep((int)(1000d / messagesPerSecond));
                var blob = cloudBlobContainer.GetBlockBlobReference($"cameras/1/{Guid.NewGuid().ToString()}.jpg");
                blob.UploadFromFileAsync(@"C:\Code\GitHub\TrafficMonitor\TrafficMonitor\TrafficMonitor\TestImages\pixel.jpg").Wait();
                Console.WriteLine("Sended image");
            }
        }
    }
}
