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
using Unity;

namespace PolygonDesigner.ViewLogic
{
    public class PolygonManagementViewModel : BindableBase
    {
        private AreaCalculator Calculator;

        public PolygonManagementViewModel(IUnityContainer container = null, AreaCalculator areaCalculator = null)
        {
            Polygons = new ObservableCollection<Polygon>();

            GenerateAndAddPolygonCommand = new DelegateCommand(
                GenerateAndAddPolygon,
                () => SelectedPolygonGenerator != null);

            Calculator = areaCalculator;
            CalculateAreaForSelectedPolygonCommand = new DelegateCommand(
                CalculateAreaForSelectedPolygon,
                () => Calculator != null && SelectedPolygon != null && !IsCalculatingArea);

            CancelAreaCalculationCommand = new DelegateCommand(
                () => CancelSource?.Cancel(),
                () => IsCalculatingArea);

            if (container != null)
            {
                Generators = GetGeneratorsFromContainer(container);
                SelectedPolygonGenerator = Generators.LastOrDefault()?.Generator;
            }
        }

        private IEnumerable<GeneratorInfo> GetGeneratorsFromContainer(IUnityContainer container) =>
            container.ResolveAll<PolygonGenerator>()
                .Select(g => new GeneratorInfo(this,
                    (g.GetType()
                        .GetCustomAttributes(typeof(FriendlyNameAttribute), true)
                        .FirstOrDefault() as FriendlyNameAttribute)?.FriendlyName ?? g.GetType().Name,
                    g));

        public const double MaxSideLength = 400d;

        public ObservableCollection<Polygon> Polygons { get; }

        public IEnumerable<GeneratorInfo> Generators { get; }

        private PolygonGenerator SelectedPolygonGeneratorValue;
        public PolygonGenerator SelectedPolygonGenerator
        {
            get { return SelectedPolygonGeneratorValue; }
            set
            {
                SetProperty(ref SelectedPolygonGeneratorValue, value);
                GenerateAndAddPolygonCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand GenerateAndAddPolygonCommand { get; }

        public DelegateCommand CalculateAreaForSelectedPolygonCommand { get; }

        public DelegateCommand CancelAreaCalculationCommand { get; }

        private Polygon SelectedPolygonValue;
        public Polygon SelectedPolygon
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
        }

        private Color GenerateRandomColor()
        {
            var random = new Random();
            return Color.FromArgb(random.Next(0, 256), random.Next(0, 256), random.Next(0, 256));
        }

        private double ProgressValue;
        public double Progress
        {
            get { return ProgressValue; }
            set { SetProperty(ref ProgressValue, value); }
        }

        private CancellationTokenSource CancelSource;

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
                RaisePropertyChanged(nameof(AreaAvailable));
            }
        }

        public bool AreaAvailable => Area.HasValue;

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
                Area = await Calculator.CalculateAreaAsync(SelectedPolygon.Points, CancelSource.Token,
                    new Progress<double>(progress => Progress = progress));
            }
            catch (OperationCanceledException)
            {
                Area = null;
            }

            IsCalculatingArea = false;
            CancelSource = null;
        }
    }
}
