using Moq;
using Polygon.Core.Generators;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Polygon.Core.Tests
{
    public class TestMonteCarloAreaCalculator
    {
        private void IsApproximately(double expected, double value) =>
            Assert.True(value >= expected - 2d && value <= expected + 2d);

        [Fact]
        public async Task Square()
        {
            const double sideLength = 20d;
            var shape = new SquarePolygonGenerator().Generate(sideLength);
            var mc = new MonteCarloAreaCalculator();
            var area = await mc.CalculateAreaAsync(shape);

            Assert.Equal(Math.Pow(sideLength, 2), area);
        }

        [Fact]
        public async Task Triangle()
        {
            const double sideLength = 20d;
            var shape = new TrianglePolygonGenerator().Generate(sideLength);
            var mc = new MonteCarloAreaCalculator(new MonteCarloAreaCalculator.Options { SimulationDuration = TimeSpan.FromMilliseconds(250) });
            var area = await mc.CalculateAreaAsync(shape);

            var expectedArea = Math.Pow(sideLength, 2) / 2d;
            IsApproximately(expectedArea, area);
        }

        [Fact]
        public async Task ComplexShape()
        {
            /*
             * ##  ##
             * ######
             */
            const double sideLength = 10d;
            Memory<Point> shape = new[] {
                new Point(0d, 0d),
                new Point(sideLength * 3, 0d),
                new Point(sideLength * 3, sideLength * 2d),
                new Point(sideLength * 2d, sideLength * 2d),
                new Point(sideLength * 2d, 10d),
                new Point(sideLength, sideLength),
                new Point(sideLength, sideLength * 2d),
                new Point(0d, sideLength * 2d)
            };

            var mc = new MonteCarloAreaCalculator(new MonteCarloAreaCalculator.Options { SimulationDuration = TimeSpan.FromMilliseconds(250) });
            var area = await mc.CalculateAreaAsync(shape);

            var expectedArea = Math.Pow(sideLength, 2) * 5;
            IsApproximately(expectedArea, area);
        }

        [Fact]
        public void InvalidOptions()
        {
            Assert.ThrowsAsync<ArgumentException>(() =>
            {
                var mc = new MonteCarloAreaCalculator();
                mc.CalculationOptions.Iterations = null;
                mc.CalculationOptions.SimulationDuration = null;
                return mc.CalculateAreaAsync(Array.Empty<Point>());
            });
        }

        [Fact]
        public void Cancel()
        {
            Assert.ThrowsAsync<OperationCanceledException>(() =>
            {
                var mc = new MonteCarloAreaCalculator();
                mc.CalculationOptions.SimulationDuration = TimeSpan.FromSeconds(1);
                var cts = new CancellationTokenSource();
                cts.CancelAfter(10);
                return mc.CalculateAreaAsync(new SquarePolygonGenerator().Generate(10d), cts.Token);
            });
        }

        [Fact]
        public async Task ProgressIterations()
        {
            const double sideLength = 20d;
            var shape = new SquarePolygonGenerator().Generate(sideLength);
            var mc = new MonteCarloAreaCalculator(new MonteCarloAreaCalculator.Options
            {
                Iterations = 10,
                ProgressReportingIterations = 2
            });

            var progresses = new List<double>();
            var progressMock = new Mock<IProgress<double>>();
            progressMock.Setup(x => x.Report(It.IsAny<double>()))
                .Callback<double>((double progress) => progresses.Add(progress));
            await mc.CalculateAreaAsync(shape, CancellationToken.None, progressMock.Object);
            Assert.Equal(new[] { 0d, 0.2d, 0.4d, 0.6d, 0.8d, 1d }, progresses);
        }

        [Fact]
        public async Task ProgressTime()
        {
            const double sideLength = 20d;
            var shape = new SquarePolygonGenerator().Generate(sideLength);
            var mc = new MonteCarloAreaCalculator(new MonteCarloAreaCalculator.Options
            {
                SimulationDuration = TimeSpan.FromMilliseconds(50),
                ProgressReportingIterations = 2
            });

            var progresses = new List<double>();
            var progressMock = new Mock<IProgress<double>>();
            progressMock.Setup(x => x.Report(It.IsAny<double>()))
                .Callback<double>((double progress) => progresses.Add(progress));
            await mc.CalculateAreaAsync(shape, CancellationToken.None, progressMock.Object);
            Assert.True(progresses.Count > 2);
            Assert.Equal(0d, progresses[0]);
            Assert.Equal(1d, progresses[progresses.Count - 1]);
        }
    }
}
