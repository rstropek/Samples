using Xunit;

namespace Polygon.Core.Tests
{
    public class TestDoubleExtensions
    {
        [Fact]
        public void NearZero()
        {
            Assert.True(0d.IsNearZero());
        }

        [Fact]
        public void NotNearZero()
        {
            Assert.False(1d.IsNearZero());
        }
    }
}
