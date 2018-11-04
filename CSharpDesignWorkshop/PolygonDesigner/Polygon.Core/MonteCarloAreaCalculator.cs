using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Polygon.Core
{
    public partial class MonteCarloAreaCalculator : AreaCalculator
    {
        public class Options
        {
            public TimeSpan? SimulationDuration { get; set; }

            public int? Iterations { get; set; }

            public int? ProgressReportingIterations { get; set; }

            public ContainmentChecker ContainmentChecker { get; set; }
        }

        public static readonly Options DefaultOptions = new Options
        {
            Iterations = 1000,
            ProgressReportingIterations = 100
        };

        public MonteCarloAreaCalculator() : this(DefaultOptions) { }

        public MonteCarloAreaCalculator(Options options) => CalculationOptions = options;

        public Options CalculationOptions { get; }

        public Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape) =>
            CalculateAreaAsync(shape, CancellationToken.None);

        public Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape, CancellationToken cancellation) =>
            CalculateAreaAsync(shape, cancellation, null);

        public Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape, CancellationToken cancellation, IProgress<double> progress)
        {
            if (!CalculationOptions.Iterations.HasValue && !CalculationOptions.SimulationDuration.HasValue)
            {
                throw new InvalidOperationException("Iterations or simulation duration must be set in calculation options");
            }

            // Copy calculation options for current run
            var calcController = new CalculationController(
                CalculationOptions.Iterations,
                CalculationOptions.SimulationDuration,
                CalculationOptions.ProgressReportingIterations,
                progress);
            var checker = CalculationOptions.ContainmentChecker ?? new RayCasting();

            return Task.Run(() =>
            {
                var localShape = shape.Span;
                (var minX, var maxX, var minY, var maxY) = GetBoundingRectangle(localShape);

                var random = new Random();
                for (calcController.Start(), calcController.Report0Perc(); calcController.Continue; calcController.Iterations++)
                {
                    var point = new Point(
                        minX + random.NextDouble() * (maxX - minX),
                        minY + random.NextDouble() * (maxY - minY));
                    if (checker.Contains(localShape, point))
                    {
                        calcController.InCounter++;
                    }

                    cancellation.ThrowIfCancellationRequested();

                    calcController.ReportProgress();
                }

                calcController.EnsureReported100Perc();
                return (maxX - minX) * (maxY - minY) * calcController.InCounter / calcController.Iterations;
            });
        }

        private static (double minX, double maxX, double minY, double maxY) GetBoundingRectangle(ReadOnlySpan<Point> shape)
        {
            var result = (
                minX: double.MaxValue,
                maxX: double.MinValue,
                minY: double.MaxValue,
                maxY: double.MinValue);
            foreach (var point in shape)
            {
                if (point.X < result.minX) { result.minX = point.X; }
                if (point.X > result.maxX) { result.maxX = point.X; }
                if (point.Y < result.minY) { result.minY = point.Y; }
                if (point.Y > result.maxY) { result.maxY = point.Y; }
            }

            return result;
        }
    }
}
