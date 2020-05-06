using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Azure.WebJobs.ServiceBus;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;
using Newtonsoft.Json;
using TrafficMonitor.Model;
using TrafficMonitor.Services;

namespace TrafficMonitorFunctionApp.Functions
{
    public class ProcessCarImage
    {
        private readonly Configuration configuration;
        private readonly ILicensePlateRecognizer licensePlateRecognizer;

        public ProcessCarImage(Configuration configuration, ILicensePlateRecognizer licensePlateRecognizer)
        {
            this.configuration = configuration;
            this.licensePlateRecognizer = licensePlateRecognizer;
        }

        /// <summary>
        /// Process incoming camera image
        /// </summary>
        /// <remarks>
        /// This function is triggered whenever a new file appears in blob storage. For that,
        /// it uses Azure Functions's Blob Trigger.
        /// The result of the function is a Service Bus message representing the license plate read.
        /// The resulting message contains a Property `ReadQuality`. It is `high` if the recognition
        /// confidence is sufficient, `low` if the confidence is too low for further processing (i.e.
        /// poor image quality), and `empty` if no license plate could be recognized at all (i.e.
        /// malfunction of the license plate detection in the camera).
        /// </remarks>
        [FunctionName("ProcessCarImage")]
        [return: ServiceBus("plate-read", Connection = "SECCTRL_SEND_PLATE_READ", EntityType = EntityType.Topic)]
        public async Task<Message> Run(
            [BlobTrigger("car-images/cameras/{camera}/{name}", Connection = "SECCTRL_CAR_IMAGES")]CloudBlockBlob imageBlob,
            string camera,
            string name,
            ILogger log,
            [DurableClient] IDurableOrchestrationClient orchestrationClient)
        {
            log.LogInformation($"Start processing of new image {name} from camera {camera}");

            // Store timestamp when image was received in blob storage
            var timestamp = imageBlob.Properties.LastModified;

            // Read image file from blob storage and convert content to Base64
            log.LogInformation("Downloading image data");
            var file = new byte[imageBlob.Properties.Length];
            await imageBlob.DownloadToByteArrayAsync(file, 0);

            // Try to recognize the license plate
            log.LogInformation("Starting license plate recognition");
            var recognitionResult = await licensePlateRecognizer.RecognizeAsync(file, configuration);

            // Move image to archive
            log.LogInformation("Moving image to archive folder");
            var archiveImageId = Guid.NewGuid().ToString();
            var archiveBlob = imageBlob.Container.GetBlockBlobReference($"archive/{archiveImageId}.jpg");
            await archiveBlob.StartCopyAsync(imageBlob);
            await imageBlob.DeleteAsync();

            log.LogInformation("Building plate read result");
            if (recognitionResult != null)
            {
                // We have recognized a license plate
                var read = new PlateRead
                {
                    ReadTimestamp = timestamp.Value.Ticks,
                    CameraID = camera,
                    LicensePlate = recognitionResult.Plate,
                    Confidence = recognitionResult.Confidence,
                    Nationality = recognitionResult.Region,
                    NationalityConfidence = recognitionResult.RegionConfidence,
                    ImageID = archiveImageId
                };

                var readQuality = "low";
                if (recognitionResult.Confidence >= 94d && recognitionResult.RegionConfidence >= 25d)
                {
                    readQuality = "high";
                }

                if (readQuality == "low")
                {
                    var instanceId = await orchestrationClient.StartNewAsync(
                        "OrchestrateRequestApproval",
                        new PlateReadApproval
                        {
                            Read = read
                        });
                    log.LogInformation($"Durable Function Ochestration started: {instanceId}");
                }

                return CreateMessage(read, readQuality);
            }
            else
            {
                // No license plate found
                var read = new EmptyPlateRead
                {
                    ReadTimestamp = timestamp.Value.Ticks,
                    CameraID = camera,
                    ImageID = archiveImageId
                };
                return CreateMessage(read, "empty");
            }
        }

        private static Message CreateMessage(EmptyPlateRead read, string quality)
        {
            var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(read));
            var message = new Message(bytes) { ContentType = "application/json" };
            message.UserProperties["ReadQuality"] = quality;
            return message;
        }
    }
}
