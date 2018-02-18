using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Builder;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using RentalManagement.Services;
using System.Net.Http;
using Moq;
using System;
using Newtonsoft.Json;
using RentalManagement.Model;
using System.Text;
using System.Net;

namespace RentalManagement.Test
{
    [TestClass]
    public class BikeRentalControllerTest
    {
        private TestServer server;
        private HttpClient client;
        private Mock<IDataAccess> dataAccessMock;
        private Mock<IRating> ratingMock;

        public BikeRentalControllerTest()
        {
            dataAccessMock = new Mock<IDataAccess>();
            dataAccessMock.Setup(framework => framework.GetDbNameAsync()).Returns(Task.FromResult("Dummy"));

            ratingMock = new Mock<IRating>();
            ratingMock.Setup(framework => framework.CalculateTotalCosts(It.IsAny<TimeSpan>())).Returns(99m);

            this.server = new TestServer(new WebHostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddScoped<IDataAccess>(_ => dataAccessMock.Object);
                    services.AddSingleton<IRating>(_ => ratingMock.Object);
                    services.AddMvc();
                })
                .Configure(app => app.UseMvc()));
            this.client = server.CreateClient();
        }

        [TestMethod]
        public async Task TestGetDbName()
        {
            var response = await this.client.GetAsync("/api/bikeRental/dbName");

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();

            Assert.AreEqual("Dummy", responseString);
            dataAccessMock.Verify(mock => mock.GetDbNameAsync(), Times.Once());
        }

        [TestMethod]
        public async Task TestEndRental()
        {
            var response = await this.client.PostAsync(
                "/api/bikeRental/end",
                new StringContent(JsonConvert.SerializeObject(new RentalEnd { CustomerID = "c", BikeID = "b" }), Encoding.UTF8, "application/json"));

            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var rentalEndResult = JsonConvert.DeserializeObject<RentalEndResult>(responseString);

            Assert.AreEqual(99m, rentalEndResult.TotalCosts);
            Assert.AreEqual("c", rentalEndResult.CustomerID);
            Assert.AreEqual("b", rentalEndResult.BikeID);
            Assert.IsTrue(rentalEndResult.EndOfRental > rentalEndResult.BeginOfRental);
            ratingMock.Verify(mock => mock.CalculateTotalCosts(It.IsAny<TimeSpan>()), Times.Once());
        }

        [TestMethod]
        public async Task TestEndRentalInvalidCustomer()
        {
            var response = await this.client.PostAsync(
                "/api/bikeRental/end",
                new StringContent(JsonConvert.SerializeObject(new RentalEnd { CustomerID = "", BikeID = "b" }), Encoding.UTF8, "application/json"));

            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
            ratingMock.Verify(mock => mock.CalculateTotalCosts(It.IsAny<TimeSpan>()), Times.Never());
        }
    }
}
