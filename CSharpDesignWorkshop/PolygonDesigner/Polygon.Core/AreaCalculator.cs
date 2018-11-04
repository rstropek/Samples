using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Polygon.Core
{
    public interface AreaCalculator
    {
        Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape);

        Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape, CancellationToken cancellation);

        Task<double> CalculateAreaAsync(ReadOnlyMemory<Point> shape, CancellationToken cancellation, IProgress<double> progress);
    }
}
