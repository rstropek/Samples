using Polygon.Core;
using Polygon.Core.Generators;
using PolygonDesigner.ViewLogic;
using Prism.Ioc;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace PolygonDesigner
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => Container.Resolve<MainWindow>();

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<PolygonGenerator, SquarePolygonGenerator>("Square");
            containerRegistry.Register<PolygonGenerator, TrianglePolygonGenerator>("Triangle");
            containerRegistry.Register<PolygonGenerator, RandomPolygonGenerator>("Random");

            containerRegistry.RegisterSingleton<PolygonManagementViewModel>();

            containerRegistry.RegisterInstance<AreaCalculator>(new MonteCarloAreaCalculator(new MonteCarloAreaCalculator.Options
            {
                SimulationDuration = TimeSpan.FromSeconds(5d),
                ProgressReportingIterations = 100000
            }));

            containerRegistry.Register<MainWindow>();
        }
    }
}
