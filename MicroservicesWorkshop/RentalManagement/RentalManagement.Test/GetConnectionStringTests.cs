using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using RentalManagement.Services;
using System;
using System.Threading.Tasks;

namespace RentalManagement.Test
{
    [TestClass]
    public class GetConnectionStringTests
    {
        [TestMethod]
        public async Task TestConnectionStringWithoutKeyVaultAndProtocol()
        {
            var keyVaultReaderMock = new Mock<IKeyVaultReader>();
            keyVaultReaderMock.Setup(framework => framework.IsAvailable).Returns(false);

            var dbServerOptions = new DbServerOptions
            {
                Server = "s",
                Database = "d",
                DbPassword = "p"
            };
            var optionsMock = new Mock<IOptions<DbServerOptions>>();
            optionsMock.Setup(framework => framework.Value).Returns(dbServerOptions);

            var da = new DataAccess(keyVaultReaderMock.Object, optionsMock.Object);
            Assert.AreEqual("Data Source=s;Initial Catalog=d;User ID=demo;Password=p", await da.GetConnectionString());
        }

        [TestMethod]
        public async Task TestConnectionStringWithoutKeyVaultAndWithProtocol()
        {
            var keyVaultReaderMock = new Mock<IKeyVaultReader>();
            keyVaultReaderMock.Setup(framework => framework.IsAvailable).Returns(false);

            var dbServerOptions = new DbServerOptions
            {
                Server = "s",
                Database = "d",
                DbPassword = "p",
                Protocol = "tcp",
                TcpPort = 1433
            };
            var optionsMock = new Mock<IOptions<DbServerOptions>>();
            optionsMock.Setup(framework => framework.Value).Returns(dbServerOptions);

            var da = new DataAccess(keyVaultReaderMock.Object, optionsMock.Object);
            Assert.AreEqual("Data Source=tcp:s,1433;Initial Catalog=d;User ID=demo;Password=p", await da.GetConnectionString());
        }

        [TestMethod]
        public async Task TestConnectionStringWithKeyVaultAndProtocol()
        {
            var keyVaultReaderMock = new Mock<IKeyVaultReader>();
            keyVaultReaderMock.Setup(framework => framework.IsAvailable).Returns(true);
            keyVaultReaderMock.Setup(framework => framework.GetSecretAsync(It.IsAny<string>())).Returns(Task.FromResult("p"));

            var dbServerOptions = new DbServerOptions
            {
                Server = "s",
                Database = "d",
                Protocol = "tcp"
            };
            var optionsMock = new Mock<IOptions<DbServerOptions>>();
            optionsMock.Setup(framework => framework.Value).Returns(dbServerOptions);

            var da = new DataAccess(keyVaultReaderMock.Object, optionsMock.Object);
            Assert.AreEqual("Data Source=tcp:s,1433;Initial Catalog=d;User ID=demo;Password=p", await da.GetConnectionString());
        }
    }
}
