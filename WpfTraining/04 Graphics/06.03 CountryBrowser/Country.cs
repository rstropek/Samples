using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Samples
{
	public class Country : Shape
	{
		public string IsoCode
		{
			get { return (string)GetValue(IsoCodeProperty); }
			set { SetValue(IsoCodeProperty, value); }
		}
		public static readonly DependencyProperty IsoCodeProperty =
			DependencyProperty.Register("IsoCode", typeof(string), typeof(Country),
			new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.AffectsRender|
				FrameworkPropertyMetadataOptions.AffectsMeasure|
				FrameworkPropertyMetadataOptions.AffectsArrange));

		protected override Geometry DefiningGeometry
		{
			get
			{
				if (IsoCode.Length > 0)
					return (PathGeometry)FindResource(IsoCode);
				else
					return new PathGeometry();
			}
		}
	}
}
