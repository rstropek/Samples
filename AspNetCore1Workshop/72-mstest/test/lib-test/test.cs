using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class TestClass
    {
        [TestMethod]
        public void TestAdd()
        {
            Assert.AreEqual(2, Library.Math.Add(1, 1));
        }
        
        [TestMethod]
        public void TestSubtract()
        {
            Assert.AreEqual(0, Library.Math.Subtract(1, 1));
        }

        // Enable this test to see it failing        
        [TestMethod]
        [Ignore]
        public void TestFailing()
        {
            Assert.AreEqual(0, Library.Math.Add(1, 1));
        }
    }
}