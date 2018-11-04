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
                () => Calculator != null && SelectedPolygon != null);

            if (container != null)
            {
                Generators = container.ResolveAll<PolygonGenerator>()
                    .Select(g => new GeneratorInfo(this,
                        (g.GetType()
                            .GetCustomAttributes(typeof(FriendlyNameAttribute), true)
                            .FirstOrDefault() as FriendlyNameAttribute)?.FriendlyName ?? g.GetType().Name,
                        g));
                SelectedPolygonGenerator = Generators.LastOrDefault()?.Generator;
            }
        }

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

        private Polygon SelectedPolygonValue;
        public Polygon SelectedPolygon
        {
            get { return SelectedPolygonValue; }
            set
            {
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

            await Calculator.CalculateAreaAsync(SelectedPolygon.Points, CancellationToken.None,
                new Progress<double>(progress => Progress = progress));
        }
    }
}
