using System;
using System.Windows;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace Samples
{
	public class WheelMesh : DependencyObject
	{
		public WheelMesh()
		{
			OnNumberOfSidesChanged(this, new DependencyPropertyChangedEventArgs());
		}

		public double Radius
		{
			get { return (double)GetValue(RadiusProperty); }
			set { SetValue(RadiusProperty, value); }
		}
		public static readonly DependencyProperty RadiusProperty =
			DependencyProperty.Register("Radius", typeof(double), typeof(WheelMesh),  
			new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender|
				FrameworkPropertyMetadataOptions.AffectsArrange|FrameworkPropertyMetadataOptions.AffectsMeasure,
				new PropertyChangedCallback(OnNumberOfSidesChanged)));

		public double Height
		{
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		public static readonly DependencyProperty HeightProperty =
			DependencyProperty.Register("Height", typeof(double), typeof(WheelMesh),  
			new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender|
				FrameworkPropertyMetadataOptions.AffectsArrange|FrameworkPropertyMetadataOptions.AffectsMeasure,
				new PropertyChangedCallback(OnNumberOfSidesChanged)));

		public bool FlatTexture
		{
			get { return (bool)GetValue(FlatTextureProperty); }
			set { SetValue(FlatTextureProperty, value); }
		}
		public static readonly DependencyProperty FlatTextureProperty =
			DependencyProperty.Register("FlatTexture", typeof(bool), typeof(WheelMesh),
			new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender |
				FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure,
				new PropertyChangedCallback(OnNumberOfSidesChanged)));

		public int NumberOfSides
		{
			get { return (int)GetValue(NumberOfSidesProperty); }
			set { SetValue(NumberOfSidesProperty, value); }
		}
		public static readonly DependencyProperty NumberOfSidesProperty =
			DependencyProperty.Register("NumberOfSides", typeof(int), typeof(WheelMesh), 
			new FrameworkPropertyMetadata(16, FrameworkPropertyMetadataOptions.AffectsRender|
				FrameworkPropertyMetadataOptions.AffectsArrange|FrameworkPropertyMetadataOptions.AffectsMeasure,
				new PropertyChangedCallback(OnNumberOfSidesChanged)));
		private static void OnNumberOfSidesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			WheelMesh wheel = (WheelMesh)d;

			Point3DCollection points = new Point3DCollection(3 + wheel.NumberOfSides - 2);
			Int32Collection triangles = new Int32Collection(wheel.NumberOfSides);
			PointCollection textureCoords = new PointCollection(3 + wheel.NumberOfSides - 2);

			// Center
			points.Add(new Point3D(wheel.Radius, wheel.Radius, wheel.Height));
			textureCoords.Add(new Point(0.5, wheel.FlatTexture ? 0.5 : 0));
			// Top
			points.Add(new Point3D(wheel.Radius, 0, 0));
			textureCoords.Add(new Point(0.5, 0));

			for (int i = 1; i < wheel.NumberOfSides; i++)
			{
				double beta = 2 * Math.PI / wheel.NumberOfSides * i;
				double alpha = (Math.PI - beta) / 2;
				double length = 2 * wheel.Radius * Math.Sin(beta / 2);
				double x = length * Math.Cos(Math.PI / 2 - alpha);
				double y = length * Math.Sin(Math.PI / 2 - alpha);

				points.Add(new Point3D(wheel.Radius + x, y, 0));
				textureCoords.Add(new Point((wheel.Radius + x) / (2*wheel.Radius), y / (2*wheel.Radius)));

				triangles.Add(0);
				triangles.Add(i);
				triangles.Add(i+1);
			}
			triangles.Add(0);
			triangles.Add(wheel.NumberOfSides);
			triangles.Add(1);
			wheel.SetValue(PointsProperty, points);
			wheel.SetValue(TriangleIndicesProperty, triangles);
			wheel.SetValue(TextureCoordinatesProperty, textureCoords);
		}

		public Point3DCollection Points
		{
			get { return (Point3DCollection)GetValue(PointsProperty); }
		}
		public static readonly DependencyProperty PointsProperty =
			DependencyProperty.Register("Points", typeof(Point3DCollection), typeof(WheelMesh),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender |
			            FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public Int32Collection TriangleIndices
		{
			get { return (Int32Collection)GetValue(PointsProperty); }
		}
		public static readonly DependencyProperty TriangleIndicesProperty =
			DependencyProperty.Register("TriangleIndices", typeof(Int32Collection), typeof(WheelMesh),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender |
						FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));

		public PointCollection TextureCoordinates
		{
			get { return (PointCollection)GetValue(TextureCoordinatesProperty); }
		}
		public static readonly DependencyProperty TextureCoordinatesProperty =
			DependencyProperty.Register("TextureCoordinates", typeof(PointCollection), typeof(WheelMesh),
			new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender |
						FrameworkPropertyMetadataOptions.AffectsArrange | FrameworkPropertyMetadataOptions.AffectsMeasure));
	}
}
