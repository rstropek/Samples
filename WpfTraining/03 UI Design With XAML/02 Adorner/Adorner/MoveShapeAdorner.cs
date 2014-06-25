using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Samples.Adorner
{
	public class MoveShapeAdorner : System.Windows.Documents.Adorner
	{
		private double offsetX = 0;
		private double offsetY = 0;
		private double mouseLeft;
		private double mouseTop;
		private Canvas canvas;

		public double OffsetX
		{
			get
			{
				return offsetX;
			}
		}

		public double OffsetY
		{
			get
			{
				return offsetY;
			}
		}

		public MoveShapeAdorner(Canvas canvas, Shape adornedElement)
			: base(adornedElement)
		{
			Point point = Mouse.GetPosition(adornedElement);
			mouseLeft = point.X;
			mouseTop = point.Y;

			this.canvas = canvas;
			canvas.MouseMove += new System.Windows.Input.MouseEventHandler(Canvas_MouseMove);
		}

		private void Canvas_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			Point point = Mouse.GetPosition(this.AdornedElement);
			if (Mouse.GetPosition(canvas).X > 0 && Mouse.GetPosition(canvas).X < canvas.ActualWidth)
			{
				offsetX = point.X - mouseLeft;
			}
			if (Mouse.GetPosition(canvas).Y > 0 && Mouse.GetPosition(canvas).Y < canvas.ActualHeight)
			{
				offsetY = point.Y - mouseTop;
			}
			this.InvalidateVisual();
		}

		protected override void OnRender(System.Windows.Media.DrawingContext drawingContext)
		{
			Rect adornedElementRect = new Rect(this.AdornedElement.DesiredSize);
			adornedElementRect.X += offsetX;
			adornedElementRect.Y += offsetY;

			SolidColorBrush renderBrush = new SolidColorBrush(Colors.LightGray);
			Pen renderPen = new Pen(new SolidColorBrush(Colors.Black), 1);
			double renderRadius = 3.0;

			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopLeft, renderRadius, renderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.TopRight, renderRadius, renderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomLeft, renderRadius, renderRadius);
			drawingContext.DrawEllipse(renderBrush, renderPen, adornedElementRect.BottomRight, renderRadius, renderRadius);
		}
	}
}
