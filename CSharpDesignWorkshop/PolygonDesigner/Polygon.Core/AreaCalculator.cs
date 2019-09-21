using System;
using System.Threading;
using System.Threading.Tasks;

namespace Polygon.Core
{
    /// <summary>
    /// Implements a class that can calculate the area of a shape
    /// </summary>
    public interface IAreaCalculator
    {
        Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape);

        Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape, CancellationToken cancellation);

        Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape, IProgress<double>? progress, CancellationToken cancellation);
    }
}
