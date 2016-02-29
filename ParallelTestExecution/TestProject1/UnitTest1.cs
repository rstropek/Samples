using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TestProject1
{
    [TestClass]
    public class UnitTest2
    {
        [TestCategory("TestProject1")]
        [TestMethod]
        public async Task TestMethod1() => await this.DummyTest();

        [TestCategory("TestProject1")]
        [TestMethod]
        public async Task TestMethod2() => await this.DummyTest();

        [TestCategory("TestProject1")]
        [TestMethod]
        public async Task TestMethod3() => await this.DummyTest();

        [TestCategory("TestProject1")]
        [TestMethod]
        public async Task TestMethod4() => await this.DummyTest();

        private async Task DummyTest([CallerMemberName] string caller = null)
        {
            Debug.WriteLine($"Starting {caller} ...");
            await Task.Delay(1000);
            Assert.AreEqual(1, 1);
            Debug.WriteLine($"Finished {caller} ...");
        }
    }
}
