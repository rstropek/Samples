using System;
using System.Collections.Generic;
using System.Text;

namespace Polygon.Core
{
    public class MonteCarloAreaCalculatorOptions
    {
        public TimeSpan? SimulationDuration { get; set; }

        public int? Iterations { get; set; }

        public int? ProgressReportingIterations { get; set; }

        public IContainmentChecker? ContainmentChecker { get; set; }
    }

}
