using System;
using Xunit;

namespace Polygon.Core.Tests
{
    public class TestRayCasting
    {
        [Fact]
        public void Rectangle()
        {
            ReadOnlySpan<Point> shape = stackalloc[] {
                new Point(0d, 0d),
                new Point(20d, 0d),
                new Point(20d, 20d),
                new Point(0d, 20d)
            };

            var rc = new RayCasting();

            Assert.True(rc.Contains(shape, new Point(10d, 10d)));
            Assert.False(rc.Contains(shape, new Point(-10d, 10d)));
            Assert.False(rc.Contains(shape, new Point(10d, 30d)));
        }

        [Fact]
        public void Triangle()
        {
            ReadOnlySpan<Point> shape = stackalloc[] {
                new Point(0d, 0d),
                new Point(20d, 0d),
                new Point(10d, 10d)
            };

            var rc = new RayCasting();

            Assert.True(rc.Contains(shape, new Point(10d, 5d)));
            Assert.False(rc.Contains(shape, new Point(0d, 5d)));
            Assert.False(rc.Contains(shape, new Point(20d, 5d)));
        }
    }
}
