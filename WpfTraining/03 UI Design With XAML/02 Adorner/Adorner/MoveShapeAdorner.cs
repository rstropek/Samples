using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Samples.Adorner
{
    public class MoveShapeAdorner : System.Windows.Documents.Adorner
	{
		private double mouseLeft;
		private double mouseTop;
		private Canvas canvas;

		public double OffsetX { get; private set; }

		public double OffsetY { get; private set; }

		public MoveShapeAdorner(Canvas canvas, Shape adornedElement)
			: base(adornedElement)
		{
			var point = Mouse.GetPosition(adornedElement);
			mouseLeft = point.X;
			mouseTop = point.Y;

			this.canvas = canvas;
			canvas.MouseMove += new System.Windows.Input.MouseEventHandler(Canvas_MouseMove);
		}

		private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var point = Mouse.GetPosition(this.AdornedElement);
			if (Mouse.GetPosition(canvas).X > 0 && Mouse.GetPosition(canvas).X < canvas.ActualWidth)
			{
				OffsetX = point.X - mouseLeft;
			}

			if (Mouse.GetPosition(canvas).Y > 0 && Mouse.GetPosition(canvas).Y < canvas.ActualHeight)
			{
				OffsetY = point.Y - mouseTop;
			}

			this.InvalidateVisual();
		}

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			var adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
			adornedElementRect.X += OffsetX;
			adornedElementRect.Y += OffsetY;

			var renderBrush = new SolidColorBrush(Colors.LightGray);
			Pen renderPen = new Pen(new SolidColorBrush(Colors.Black), 1);
			double renderRadius = 3.0;

			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
		}
	}
}
