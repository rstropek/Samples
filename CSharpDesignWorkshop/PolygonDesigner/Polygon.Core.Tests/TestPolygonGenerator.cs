using Polygon.Core.Generators;
using System;
using System.Collections.Generic;
using Xunit;

namespace Polygon.Core.Tests
{
    public class TestPolygonGenerator
    {
        public static readonly IEnumerable<object[]> generators = new[]
        {
            new object[] { new RandomPolygonGenerator() },
            new object[] { new SquarePolygonGenerator() },
            new object[] { new TrianglePolygonGenerator() }
        };

        // Note: xUnit Theory instead of Fact

        [Theory]
        [MemberData(nameof(generators))]
        public void MaxSideLength(IPolygonGenerator generator)
        {
            if (generator == null)
            {
                throw new ArgumentNullException(nameof(generator));
            }

            const double maxSideLength = 100d;

            // Generate polygon
            var points = generator.Generate(maxSideLength);

            // Size of polygon must be <= specified max side length
            foreach (var point in points.Span)
            {
                Assert.True(point.X <= maxSideLength);
                Assert.True(point.Y <= maxSideLength);
            }
        }
    }
}
