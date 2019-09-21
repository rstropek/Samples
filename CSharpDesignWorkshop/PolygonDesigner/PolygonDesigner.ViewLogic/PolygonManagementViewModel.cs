using Polygon.Core;
using Polygon.Core.Generators;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;

namespace PolygonDesigner.ViewLogic
{
    public class PolygonManagementViewModel : BindableBase, IDisposable
    {
        private readonly IAreaCalculator? Calculator;
        private readonly IPolygonClipper? Clipper;

        public PolygonManagementViewModel(IEnumerable<IPolygonGenerator> generators, IAreaCalculator? areaCalculator = null, IPolygonClipper? clipper = null)
        {
            Polygons = new ObservableCollection<Polygon>();

            GenerateAndAddPolygonCommand = new DelegateCommand(
                GenerateAndAddPolygon,
                () => SelectedPolygonGenerator != null);

            Clipper = clipper;
            ClipPolygonsCommand = new DelegateCommand(
                ClipPolygons,
                () => Clipper != null && !IsCalculatingArea && Polygons.Count > 1);

            Calculator = areaCalculator;
            CalculateAreaForSelectedPolygonCommand = new DelegateCommand(
                CalculateAreaForSelectedPolygon,
                () => Calculator != null && SelectedPolygon != null && !IsCalculatingArea);

            CancelAreaCalculationCommand = new DelegateCommand(
                () => CancelSource?.Cancel(),
                () => IsCalculatingArea);

            if (generators != null)
            {
                Generators = GetGeneratorsFromContainer(generators);
                SelectedPolygonGenerator = Generators?.LastOrDefault()?.Generator;
            }
        }

        ~PolygonManagementViewModel()
        {
            Dispose(false);
        }

        private IEnumerable<GeneratorInfo> GetGeneratorsFromContainer(IEnumerable<IPolygonGenerator> generators) =>
            generators.Select(g => new GeneratorInfo(this,
                (g.GetType()
                    .GetCustomAttributes(typeof(FriendlyNameAttribute), true)
                    .FirstOrDefault() as FriendlyNameAttribute)?.FriendlyName ?? g.GetType().Name,
                g));

        public const double MaxSideLength = 400d;

        public ObservableCollection<Polygon> Polygons { get; }

        public IEnumerable<GeneratorInfo>? Generators { get; }

        private IPolygonGenerator? SelectedPolygonGeneratorValue;
        public IPolygonGenerator? SelectedPolygonGenerator
        {
            get { return SelectedPolygonGeneratorValue; }
            set
            {
                SetProperty(ref SelectedPolygonGeneratorValue, value);
                GenerateAndAddPolygonCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand GenerateAndAddPolygonCommand { get; }

        public DelegateCommand ClipPolygonsCommand { get; }

        public DelegateCommand CalculateAreaForSelectedPolygonCommand { get; }

        public DelegateCommand CancelAreaCalculationCommand { get; }

        private Polygon? SelectedPolygonValue;
        public Polygon? SelectedPolygon
        {
            get { return SelectedPolygonValue; }
            set
            {
                if (IsCalculatingArea)
                {
                    CancelSource?.Cancel();
                }

                SetProperty(ref SelectedPolygonValue, value);
                CalculateAreaForSelectedPolygonCommand.RaiseCanExecuteChanged();
            }
        }

        private void GenerateAndAddPolygon()
        {
            if (SelectedPolygonGenerator == null)
            {
                throw new InvalidOperationException($"No polygon generator selected");
            }

            var polygon = SelectedPolygonGenerator.Generate(MaxSideLength);
            var color = GenerateRandomColor();
            Polygons.Add(new Polygon
            {
                Points = polygon,
                Description = $"Auto-generated Polygon at {DateTime.Now:D}",
                StrokeColor = color,
                FillColor = Color.FromArgb(128, color)
            });

            ClipPolygonsCommand.RaiseCanExecuteChanged();
        }

        private void ClipPolygons()
        {
            if (Polygons.Count > 1 && Clipper != null)
            {
                var clippedPolyPoints = Polygons[0].Points.Span;
                for (var i = 1; i < Polygons.Count; i++)
                {
                    clippedPolyPoints = Clipper.GetIntersectedPolygon(clippedPolyPoints, Polygons[i].Points.Span);
                }

                var color = GenerateRandomColor();
                Polygons.Add(new Polygon
                {
                    Points = new ReadOnlyMemory<global::Polygon.Core.Point>(clippedPolyPoints.ToArray()),
                    Description = $"Clipped Polygon at {DateTime.Now:D}",
                    StrokeColor = color,
                    FillColor = Color.FromArgb(128, color)
                });
            }
        }

        private static Color GenerateRandomColor()
        {
            var random = new Random();
            return Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
        }

        private double ProgressValue;
        public double Progress
        {
            get { return ProgressValue; }
            private set { SetProperty(ref ProgressValue, value); }
        }

        private CancellationTokenSource? CancelSource;

        private bool IsCalculatingAreaValue;
        public bool IsCalculatingArea
        {
            get { return IsCalculatingAreaValue; }
            private set
            {
                SetProperty(ref IsCalculatingAreaValue, value);
                CalculateAreaForSelectedPolygonCommand.RaiseCanExecuteChanged();
                CancelAreaCalculationCommand.RaiseCanExecuteChanged();
            }
        }

        private double? AreaValue;
        public double? Area
        {
            get { return AreaValue; }
            private set
            {
                SetProperty(ref AreaValue, value);
                RaisePropertyChanged(nameof(IsAreaAvailable));
            }
        }

        public bool IsAreaAvailable => Area.HasValue;

        private async void CalculateAreaForSelectedPolygon()
        {
            if (Calculator == null)
            {
                throw new InvalidOperationException("No calculator available");
            }

            if (SelectedPolygon == null)
            {
                throw new InvalidOperationException("No polygon selected");
            }

            if (IsCalculatingArea)
            {
                throw new InvalidOperationException("Calculation is already running");
            }

            CancelSource = new CancellationTokenSource();
            Area = null;
            IsCalculatingArea = true;
            try
            {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                Area = await Calculator.CalculateAreaAsync(SelectedPolygon.Points,
                    new Progress<double>(progress => Progress = progress),
                    CancelSource.Token);
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            }
            catch (OperationCanceledException)
            {
                Area = null;
            }

            IsCalculatingArea = false;
            CancelSource = null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing && CancelSource != null)
            {
                CancelSource.Dispose();
            }
        }
    }
}
