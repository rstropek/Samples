using Polygon.Core.Generators;
using System;
using System.Linq;
using System.Reflection;
using Xunit;

namespace Polygon.Core.Tests
{
    public class TestPolygonGenerator
    {
        [Fact]
        public void MaxSideLength()
        {
            const double maxSideLength = 100d;

            // Find all polygon generators
            var generatorTypes = Assembly.GetAssembly(typeof(PolygonGenerator))
                .DefinedTypes
                .Where(t => !t.IsAbstract && typeof(PolygonGenerator).GetTypeInfo().IsAssignableFrom(t));

            foreach(var gt in generatorTypes)
            {
                // Create instande of polygon generator
                var generator = (PolygonGenerator)Activator.CreateInstance(gt);

                // Generate polygon
                var points = generator.Generate(maxSideLength);

                // Size of polygon must be <= specified max side length
                foreach(var point in points.Span)
                {
                    Assert.True(point.X <= maxSideLength);
                    Assert.True(point.Y <= maxSideLength);
                }
            }
        }
    }
}
