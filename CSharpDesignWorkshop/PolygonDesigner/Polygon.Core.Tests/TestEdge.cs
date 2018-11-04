using Xunit;

namespace Polygon.Core.Tests
{
    public class TestEdge
    {
        private static readonly Edge E = new Edge(new Point(1d, 1d), new Point(2d, 2d));
        private static readonly Edge EEqual = new Edge(new Point(1d, 1d), new Point(2d, 2d));
        private static readonly Edge ENotEqual = new Edge(new Point(2d, 2d), new Point(1d, 1d));

        [Fact]
        public void Equatable()
        {
            Assert.True(E.Equals(EEqual));
            Assert.False(E.Equals(ENotEqual));
        }

        [Fact]
        public void Operators()
        {
            Assert.True(E == EEqual);
            Assert.True(E != ENotEqual);
        }

        [Fact]
        public void Equal()
        {
            Assert.True(E.Equals((object)EEqual));
            Assert.False(E.Equals((object)ENotEqual));
        }

        [Fact]
        public void TestGetHashCode()
        {
            Assert.Equal(E.GetHashCode(), EEqual.GetHashCode());
            Assert.NotEqual(E.GetHashCode(), ENotEqual.GetHashCode());
        }

        [Fact]
        public void Deconstruct()
        {
            (var from, var to) = E;
            Assert.Equal(from, E.From);
            Assert.Equal(to, E.To);
        }

        [Fact]
        public void GetIntersectionPoint()
        {
            var e1 = new Edge(new Point(0d, 0d), new Point(2d, 2d));
            Assert.Equal(new Point(1d, 1d), e1.GetIntersectionPoint(new Point(0d, 2d), new Point(2d, 0d)));
        }

        [Fact]
        public void GetIntersectionPointOutside()
        {
            var e1 = new Edge(new Point(0d, 0d), new Point(1d, 1d));
            Assert.Equal(new Point(2d, 2d), e1.GetIntersectionPoint(new Point(0d, 4d), new Point(1d, 3d)));
        }

        [Fact]
        public void GetIntersectionPointParallelEdges()
        {
            var e1 = new Edge(new Point(0d, 0d), new Point(2d, 2d));
            Assert.Null(e1.GetIntersectionPoint(new Point(1d, 1d), new Point(3d, 3d)));
        }

        [Fact]
        public void IsLeftOf()
        {
            var e1 = new Edge(new Point(1d, 0d), new Point(1d, 2d));
            Assert.True(e1.IsLeftOf(new Point(0d, 1d)));
        }

        [Fact]
        public void IsNotLeftOf()
        {
            var e1 = new Edge(new Point(1d, 0d), new Point(1d, 2d));
            Assert.False(e1.IsLeftOf(new Point(2d, 1d)));
        }

        [Fact]
        public void IsLeftOfColinear()
        {
            var e1 = new Edge(new Point(1d, 0d), new Point(1d, 2d));
            Assert.Null(e1.IsLeftOf(new Point(1d, 1d)));
        }
    }
}
