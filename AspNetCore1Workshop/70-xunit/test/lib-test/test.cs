
using Xunit;

namespace Tests
{
    public class TestClass
    {
        [Fact]
        public void TestAdd()
        {
            Assert.Equal(2, Library.Math.Add(1, 1));
        }
        
        [Fact]
        public void TestSubtract()
        {
            Assert.Equal(0, Library.Math.Subtract(1, 1));
        }
        
        [Fact(Skip = "Enable this test to see it failing")]
        public void TestFailing()
        {
            Assert.Equal(0, Library.Math.Add(1, 1));
        }
    }
}