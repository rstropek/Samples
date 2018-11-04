using Polygon.Core;
using Polygon.Core.Generators;
using PolygonDesigner.ViewLogic;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PolygonDesigner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow(PolygonManagementViewModel viewModel, IRegionManager regionManager)
        {
            regionManager.RegisterViewWithRegion(RegionNames.GraphicalViewer, typeof(PolygonsViewer));
            regionManager.RegisterViewWithRegion(RegionNames.MainMenu, typeof(MainMenu));
            regionManager.RegisterViewWithRegion(RegionNames.PolygonList, typeof(PolygonsList));
            regionManager.RegisterViewWithRegion(RegionNames.PolygonDetails, typeof(PolygonDetails));

            InitializeComponent();
            //this.DataContext = this;
            this.DataContext = viewModel;

            //((PolygonManagementViewModel)this.DataContext).SelectedPolygonGenerator =
            //    new RandomPolygonGenerator();
        }

        //public string PolygonPath { get; set; }
        //public string ClipPath { get; set; }
        //public string ResultPath { get; set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //private void OnGenerate(object sender, RoutedEventArgs e)
        //{
        //    PolygonGenerator generator = new RandomPolygonGenerator();
        //    var polygon = generator.Generate(150d).Span;
        //    PolygonPath = PathMarkupConverter.Convert(polygon);

        //    generator = new TrianglePolygonGenerator();
        //    var clip = generator.Generate(150d).Span;
        //    ClipPath = PathMarkupConverter.Convert(clip);

        //    var clippedPoly = new SutherlandHodgman().GetIntersectedPolygon(polygon, clip);
        //    ResultPath = PathMarkupConverter.Convert(clippedPoly);

        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PolygonPath)));
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClipPath)));
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultPath)));
        //}
    }
}