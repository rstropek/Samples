using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polygon.Core;
using Polygon.Core.Generators;
using PolygonDesigner.ViewLogic;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace PolygonDesigner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IHost host;

        public App()
        {
            host = new HostBuilder()
                .ConfigureServices((context, services) =>
                {
                    services.AddTransient<IPolygonGenerator, SquarePolygonGenerator>();
                    services.AddTransient<IPolygonGenerator, TrianglePolygonGenerator>();
                    services.AddTransient<IPolygonGenerator, RandomPolygonGenerator>();

                    services.AddSingleton<PolygonManagementViewModel>();

                    services.AddSingleton<IPolygonClipper, SutherlandHodgman>();

                    services.AddSingleton<IAreaCalculator>(new MonteCarloAreaCalculator(new MonteCarloAreaCalculatorOptions
                    {
                        SimulationDuration = TimeSpan.FromSeconds(5d),
                        ProgressReportingIterations = 100000
                    }));

                    services.AddSingleton<MainWindow>();
                })
                .Build();
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
            await host.StartAsync();
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task

            var mainWindow = host.Services.GetService<MainWindow>();
            mainWindow.Show();
        }

        private async void Application_Exit(object sender, ExitEventArgs e)
        {
            using (host)
            {
#pragma warning disable CA2007 // Consider calling ConfigureAwait on the awaited task
                await host.StopAsync(TimeSpan.FromSeconds(5));
#pragma warning restore CA2007 // Consider calling ConfigureAwait on the awaited task
            }
        }
    }
}
