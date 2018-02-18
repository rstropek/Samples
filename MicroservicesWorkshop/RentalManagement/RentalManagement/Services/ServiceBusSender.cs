using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using RentalManagement.Model;
using System.Text;
using System.Threading.Tasks;

namespace RentalManagement.Services
{
    public class ServiceBusSender : IServiceBusSender
    {
        private IKeyVaultReader keyVaultReader;

        public ServiceBusSender(IKeyVaultReader keyVaultReader)
        {
            this.keyVaultReader = keyVaultReader;
        }

        public async Task SendRentalEndResult(RentalEndResult result)
        {
            var topicClient = new TopicClient(await keyVaultReader.GetSecretAsync("ServiceBus"), "bike-rental-end");
            var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(result)));
            await topicClient.SendAsync(message);
        }
    }
}
