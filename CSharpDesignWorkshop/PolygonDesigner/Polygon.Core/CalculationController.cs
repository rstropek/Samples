using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Polygon.Core
{
    public partial class MonteCarloAreaCalculator
    {
        private class CalculationController
        {
            private int? MaxIterations { get; }
            private TimeSpan? MaxTime { get; }
            private int? ProgressReportingIterations { get; }
            private IProgress<double> ProgressCallback { get; }
            public int InCounter { get; set; }
            public int Iterations { get; set; }
            private Stopwatch Watch { get; }
            private bool HasReported0Perc { get; set; }
            private bool HasReported100Perc { get; set; }

            public CalculationController(int? maxIterations, TimeSpan? maxTime,
                int? progressReportingIterations, IProgress<double> progressCallback)
            {
                MaxIterations = maxIterations;
                MaxTime = maxTime;
                ProgressReportingIterations = progressReportingIterations;
                ProgressCallback = progressCallback;
                if (MaxTime.HasValue)
                {
                    Watch = new Stopwatch();
                }
            }

            public void Start()
            {
                InCounter = Iterations = 0;
                HasReported0Perc = HasReported100Perc = false;
                if (MaxTime.HasValue)
                {
                    Watch.Restart();
                }
            }

            public bool Continue => (MaxIterations.HasValue && Iterations < MaxIterations.Value)
                    || (MaxTime.HasValue && Watch.Elapsed <= MaxTime.Value);

            public bool ShouldReportProgress => ProgressCallback != null
                        && ProgressReportingIterations.HasValue
                        && Iterations % ProgressReportingIterations.Value == 0;

            public void ReportProgress(double? progress = null)
            {
                var reportedProgress = progress ?? Progress;
                if (ShouldReportProgress && !(reportedProgress == 0d && HasReported0Perc) && !(reportedProgress == 1d && HasReported100Perc))
                {
                    ProgressCallback.Report(reportedProgress);
                    if (reportedProgress == 1d)
                    {
                        HasReported100Perc = true;
                    }

                    if (reportedProgress == 0d)
                    {
                        HasReported0Perc = true;
                    }
                }
            }

            public void Report0Perc()
            {
                if (!HasReported0Perc)
                {
                    ProgressCallback?.Report(0d);
                    HasReported0Perc = true;
                }
            }

            public void EnsureReported100Perc()
            {
                if (!HasReported100Perc)
                {
                    ProgressCallback?.Report(1d);
                    HasReported100Perc = true;
                }
            }

            public double Progress
            {
                get
                {
                    if (MaxIterations.HasValue)
                    {
                        return Convert.ToDouble(Iterations) / Convert.ToDouble(MaxIterations.Value);
                    }
                    else
                    {
                        return Convert.ToDouble(Watch.ElapsedMilliseconds) / Convert.ToDouble(MaxTime.Value.TotalMilliseconds);
                    }
                }
            }
        }
    }
}
