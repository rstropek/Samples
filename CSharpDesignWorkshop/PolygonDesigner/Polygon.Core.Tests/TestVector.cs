using Xunit;

namespace Polygon.Core.Tests
{
    public class TestVector
    {
        private static readonly Vector V = new Vector(1d, 2d);
        private static readonly Vector VEqual = new Vector(1d, 2d);
        private static readonly Vector VNotEqual = new Vector(2d, 1d);

        [Fact]
        public void Equatable()
        {
            Assert.True(V.Equals(VEqual));
            Assert.False(V.Equals(VNotEqual));
        }

        [Fact]
        public void Operators()
        {
            Assert.True(V == VEqual);
            Assert.True(V != VNotEqual);
        }

        [Fact]
        public void Equal()
        {
            Assert.True(V.Equals((object)VEqual));
            Assert.False(V.Equals((object)VNotEqual));
        }

        [Fact]
        public void TestGetHashCode()
        {
            Assert.Equal(V.GetHashCode(), VEqual.GetHashCode());
            Assert.NotEqual(V.GetHashCode(), VNotEqual.GetHashCode());
        }

        [Fact]
        public void Deconstruct()
        {
            (var x, var y) = V;
            Assert.Equal(x, V.X);
            Assert.Equal(y, V.Y);
        }

        [Fact]
        public void Multiplication()
        {
            Assert.Equal(new Vector(2d, 4d), 2d * V);
        }
    }
}
