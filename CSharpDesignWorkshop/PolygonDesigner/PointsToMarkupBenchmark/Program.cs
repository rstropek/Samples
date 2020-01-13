using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Running;
using Polygon.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace PointsToMarkupBenchmark
{
    class Program
    {
        static void Main()
        {
            BenchmarkRunner.Run<MarkupToPoints>();
            BenchmarkRunner.Run<PointsToMarkup>();
        }
    }

    [SimpleJob(RunStrategy.ColdStart, targetCount: 5, invocationCount: 1000)]
    [MemoryDiagnoser]
    public class MarkupToPoints
    {
        [ParamsSource(nameof(Markups))]
        public string PathMarkup { get; set; } = string.Empty;

        public IEnumerable<string> Markups { get; set; }

        public MarkupToPoints()
        {
            var markups = new[]
            {
                "M0,0Z",
                "M0,0L10,0L10,10L0,10Z",
                ""
            };

            var sb = new StringBuilder();
            for (var i = 0; i < 1000; i++)
            {
                sb.Append(i == 0 ? "M" : "L");
                sb.Append($"{i},{i}");
            }
            sb.Append("Z");
            markups[2] = sb.ToString();
            Markups = markups;
        }

        [Benchmark]
        public void StringToPoints() => PathMarkupConverter.Convert(PathMarkup);

        [Benchmark]
        public void StringToPointsOld() => PathMarkupConverter.ConvertOld(PathMarkup);
    }


    [SimpleJob(RunStrategy.ColdStart, targetCount: 5, invocationCount: 1000)]
    [MemoryDiagnoser]
    public class PointsToMarkup
    {
        [ParamsSource(nameof(PointsData))]
        public ReadOnlyMemory<Point> Points { get; set; }

        public IEnumerable<ReadOnlyMemory<Point>> PointsData { get; set; }

        public PointsToMarkup()
        {
            var points = new[]
            {
                PathMarkupConverter.Convert("M0,0Z"),
                PathMarkupConverter.Convert("M0,0L10,0L10,10L0,10Z"),
                null
            };

            var sb = new StringBuilder();
            for (var i = 0; i < 1000; i++)
            {
                sb.Append(i == 0 ? "M" : "L");
                sb.Append($"{i},{i}");
            }
            sb.Append("Z");
            points[2] = PathMarkupConverter.Convert(sb.ToString());
            PointsData = points;
        }

        [Benchmark]
        public void PointsToString() => PathMarkupConverter.Convert(Points);

        [Benchmark]
        public void PointsToStringOld() => PathMarkupConverter.ConvertOld(Points.Span);
    }
}
