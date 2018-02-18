using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RentalManagement.Services;
using System.Net.Http;
using Moq;

namespace RentalManagement.Test
{
    [TestClass]
    public class BikeRentalControllerTest
    {
        private TestServer server;
        private HttpClient client;
        private Mock<IDataAccess> dataAccessMock;

        public BikeRentalControllerTest()
        {
            dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(framework => framework.GetDbNameAsync()).Returns(Task.FromResult("Dummy"));
            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddScoped<IDataAccess>(_ => dataAccessMock.Object);
                    services.AddMvc();
                })
                .Configure(app => app.UseMvc()));
            this.client = server.CreateClient();
        }

        [TestMethod]
        public async Task TestGetDbName()
        {
            var response = await this.client.GetAsync("/api/bikeRental");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Dummy", responseString);
            dataAccessMock.Verify(mock => mock.GetDbNameAsync(), Times.Once());
        }
    }
}
