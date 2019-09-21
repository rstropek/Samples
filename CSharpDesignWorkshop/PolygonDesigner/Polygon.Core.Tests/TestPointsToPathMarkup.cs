using Polygon.Core.Generators;
using System;
using Xunit;

namespace Polygon.Core.Tests
{
    public class TestPointsToPathMarkup
    {
        [Fact]
        public void SquareToMarkup()
        {
            var square = new SquarePolygonGenerator().Generate(10d);
            Assert.Equal("M0,0L10,0L10,10L0,10Z", PathMarkupConverter.Convert(square));
        }

        [Fact]
        public void PointToMarkup()
        {
            Assert.Equal("M0,0Z", PathMarkupConverter.Convert(
                new ReadOnlyMemory<Point>(new[] { new Point(0d, 0d) })));
        }

        [Fact]
        public void EmptyPolygonToEmptyString()
        {
            Assert.Equal(string.Empty, PathMarkupConverter.Convert(new ReadOnlyMemory<Point>(Array.Empty<Point>())));
        }

        [Fact]
        public void Roundtrip()
        {
            const string markup = "M0,0L10,0L10,10L0,10Z";
            var square = PathMarkupConverter.Convert(markup);
            var squareMarkup = PathMarkupConverter.Convert(square);
            Assert.Equal(markup, squareMarkup);
        }

        [Fact]
        public void EmptyStringToEmptyPolygon()
        {
            Assert.True(PathMarkupConverter.Convert(string.Empty).Length == 0);
        }

        [Fact]
        public void ExceptionOnInvalidMarkup()
        {
            Assert.Throws<ArgumentException>(() => PathMarkupConverter.Convert("FooBar"));
        }
    }
}
