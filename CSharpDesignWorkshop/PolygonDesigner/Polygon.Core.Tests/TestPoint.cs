using Xunit;

namespace Polygon.Core.Tests
{
    public class TestPoint
    {
        private static readonly Point P = new Point(1d, 2d);
        private static readonly Point PEqual = new Point(1d, 2d);
        private static readonly Point PNotEqual = new Point(2d, 1d);

        [Fact]
        public void Equatable()
        {
            Assert.True(P.Equals(PEqual));
            Assert.False(P.Equals(PNotEqual));
        }

        [Fact]
        public void Operators()
        {
            Assert.True(P == PEqual);
            Assert.True(P != PNotEqual);
        }

        [Fact]
        public void Equal()
        {
            Assert.True(P.Equals((object)PEqual));
            Assert.False(P.Equals((object)PNotEqual));
        }

        [Fact]
        public void TestGetHashCode()
        {
            Assert.Equal(P.GetHashCode(), PEqual.GetHashCode());
            Assert.NotEqual(P.GetHashCode(), PNotEqual.GetHashCode());
        }

        [Fact]
        public void Deconstruct()
        {
            (var x, var y) = P;
            Assert.Equal(x, P.X);
            Assert.Equal(y, P.Y);
        }

        [Fact]
        public void Subtract()
        {
            var p1 = new Point(2d, 2d);
            var p2 = new Point(1d, 1d);
            Assert.Equal(new Vector(1d, 1d), p1 - p2);
        }

        [Fact]
        public void Add()
        {
            var p1 = new Point(2d, 2d);
            var p2 = new Vector(1d, 1d);
            Assert.Equal(new Point(3d, 3d), p1 + p2);
        }
    }
}
