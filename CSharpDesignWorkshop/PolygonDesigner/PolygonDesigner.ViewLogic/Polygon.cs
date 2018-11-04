using Prism.Mvvm;
using System;
using System.Drawing;
using PolygonCore = Polygon.Core;

namespace PolygonDesigner.ViewLogic
{
    public class Polygon : BindableBase
    {
        private ReadOnlyMemory<PolygonCore.Point> PointsValue;
        public ReadOnlyMemory<PolygonCore.Point> Points
        {
            get { return PointsValue; }
            set { SetProperty(ref PointsValue, value); }
        }

        private string DescriptionValue;
        public string Description
        {
            get { return DescriptionValue; }
            set { SetProperty(ref DescriptionValue, value); }
        }

        private Color StrokeColorValue;
        public Color StrokeColor
        {
            get { return StrokeColorValue; }
            set { SetProperty(ref StrokeColorValue, value); }
        }

        private Color FillColorValue;
        public Color FillColor
        {
            get { return FillColorValue; }
            set { SetProperty(ref FillColorValue, value); }
        }
    }
}
