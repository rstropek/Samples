using System;
using Xunit;

namespace Polygon.Core.Tests
{
    public class TestSutherlandHodgeman
    {
        [Fact]
        public void TooFewPointSubjectPolygon()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ReadOnlySpan<Point> dummyPoints = stackalloc Point[3];
                new SutherlandHodgman().GetIntersectedPolygon(Array.Empty<Point>(), dummyPoints);
            });
        }

        [Fact]
        public void TooFewPointClippingPolygon()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                ReadOnlySpan<Point> dummyPoints = stackalloc Point[3];
                new SutherlandHodgman().GetIntersectedPolygon(dummyPoints, Array.Empty<Point>());
            });
        }

        [Fact]
        public void ClipRectangle()
        {
            ReadOnlySpan<Point> subject = stackalloc Point[4] { new Point(0d, 0d), new Point(2d, 0d), new Point(2d, 2d), new Point(0d, 2d) };
            ReadOnlySpan<Point> clipping = stackalloc Point[4] { new Point(1d, 0d), new Point(3d, 0d), new Point(3d, 2d), new Point(1d, 2d) };
            ReadOnlySpan<Point> result = stackalloc Point[4] { new Point(1d, 2d), new Point(2d, 2d), new Point(2d, 0d), new Point(1d, 0d) };

            var intersect = new SutherlandHodgman().GetIntersectedPolygon(subject, clipping);
            Assert.Equal(result.Length, intersect.Length);
            for (var i = 0; i < intersect.Length; i++)
            {
                Assert.Equal(result[i], intersect[i]);
            }
        }

        [Fact]
        public void ClipEmpty()
        {
            ReadOnlySpan<Point> subject = stackalloc Point[4] { new Point(0d, 0d), new Point(2d, 0d), new Point(2d, 2d), new Point(0d, 2d) };
            ReadOnlySpan<Point> clipping = stackalloc Point[4] { new Point(3d, 0d), new Point(5d, 0d), new Point(5d, 2d), new Point(3d, 2d) };

            Assert.Equal(0, new SutherlandHodgman().GetIntersectedPolygon(subject, clipping).Length);
        }
    }
}
