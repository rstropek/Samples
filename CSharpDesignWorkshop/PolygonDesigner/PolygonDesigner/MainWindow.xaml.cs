using Polygon.Core;
using Polygon.Core.Generators;
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
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public string PolygonPath { get; set; }
        public string ClipPath { get; set; }
        public string ResultPath { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnGenerate(object sender, RoutedEventArgs e)
        {
            var converter = new PointsToPathMarkup();

            PolygonGenerator generator = new RandomPolygonGenerator();
            var polygon = generator.Generate(150d).Span;
            PolygonPath = converter.Convert(polygon);

            generator = new TrianglePolygonGenerator();
            var clip = generator.Generate(150d).Span;
            ClipPath = converter.Convert(clip);

            var clippedPoly = new SutherlandHodgman().GetIntersectedPolygon(polygon, clip);
            ResultPath = converter.Convert(clippedPoly);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PolygonPath)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ClipPath)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ResultPath)));
        }
    }
}