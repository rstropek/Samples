using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace Samples.Adorner
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{
		private Shape clickSource = null;

		public Window1()
		{
			InitializeComponent();
		}

		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonDown(e);

			if (e.OriginalSource is Shape)
			{
				clickSource = (Shape)e.OriginalSource;
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(clickSource);
				adornerLayer.Add(new MoveShapeAdorner(moveElementsCanvas, clickSource));
				Mouse.Capture(clickSource);
			}
		}

		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			base.OnMouseLeftButtonUp(e);

			if (clickSource != null)
			{
				AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(clickSource);
				MoveShapeAdorner adorner = (MoveShapeAdorner)adornerLayer.GetAdorners(clickSource)[0];

				Point point = Mouse.GetPosition(moveElementsCanvas);
				Canvas.SetLeft(clickSource, Canvas.GetLeft(clickSource) + adorner.OffsetX);
				Canvas.SetTop(clickSource, Canvas.GetTop(clickSource) + adorner.OffsetY);

				adornerLayer.Remove(adorner);
				clickSource = null;
				Mouse.Capture(null);
			}
		}
	}
}