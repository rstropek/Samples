using System;
using System.Windows;
using System.ComponentModel;

namespace Samples
{
	public partial class RatioPieChart : System.Windows.Controls.UserControl
	{
		public RatioPieChart()
		{
			InitializeComponent();
			DataContext = this;
		}

		#region "Ratio" dependency property
		public double Ratio
		{
			get { return (double)GetValue(RatioProperty); }
			set { SetValue(RatioProperty, value); }
		}
		public static readonly DependencyProperty RatioProperty =
			DependencyProperty.Register("Ratio", typeof(double), typeof(RatioPieChart),
			new PropertyMetadata(new PropertyChangedCallback(OnRatioPropertyChanged)));
		public static void OnRatioPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			RatioPieChart o = (RatioPieChart)d;
			// calculate end point of arc
			o.SetValue(PointProperty, new Point(50+Math.Cos((90 + 360 * o.Ratio)*Math.PI/180) * -50,
				50 - Math.Sin((90 + 360 * o.Ratio) * Math.PI / 180) * 50));
			// calculate large arc flags
			o.SetValue(IsLargeArcRatioProperty, o.Ratio >= 0.5 ? true : false);
			o.SetValue(IsLargeArcRatioRest, o.Ratio < 0.5 ? true : false);
		}
		#endregion

		#region "Point" dependency property
		public Point Point
		{
			get { return (Point)GetValue(PointProperty); }
		}
		public static readonly DependencyProperty PointProperty =
			DependencyProperty.Register("Point", typeof(Point), typeof(RatioPieChart),
			new PropertyMetadata(new Point(50,0)));
		#endregion

		#region "IsLargeArcRatio" and "IsLargeArcRest" dependency properties
		public Boolean IsLargeArcRatio
		{
			get { return (Boolean)GetValue(IsLargeArcRatioProperty); }
		}
		public static readonly DependencyProperty IsLargeArcRatioProperty =
			DependencyProperty.Register("IsLargeArcRatio", typeof(Boolean), typeof(RatioPieChart), 
			new PropertyMetadata(false));

		public Boolean IsLargeArcRest
		{
			get { return (Boolean)GetValue(IsLargeArcRatioRest); }
		}
		public static readonly DependencyProperty IsLargeArcRatioRest =
			DependencyProperty.Register("IsLargeArcRest", typeof(Boolean), typeof(RatioPieChart),
			new PropertyMetadata(true));
		#endregion
	}
}