using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfApplication1
{
	public class StepPanel : Panel
	{
		protected override Size MeasureOverride(Size availableSize)
		{
			var internalUiChildren = this.InternalChildren.Cast<UIElement>();

			var result = new Size(0, 0);
			foreach (var child in internalUiChildren)
			{
				child.Measure(availableSize);
				result.Width += child.DesiredSize.Width;
				result.Height += child.DesiredSize.Height;
			}

			return new Size(
				Math.Min(result.Width, availableSize.Width),
				Math.Min(result.Height, availableSize.Height));
		}

		protected override Size ArrangeOverride(Size finalSize)
		{
			if (this.InternalChildren.Count == 0)
			{
				return new Size(0, 0);
			}

			var internalUiChildren = this.InternalChildren.Cast<UIElement>().ToArray();
			var lastUiChild = internalUiChildren.Last();
			var widthToDistribute = finalSize.Width-lastUiChild.DesiredSize.Width;
			var heightToDistribute = finalSize.Height-lastUiChild.DesiredSize.Height;

			for (var i = 0; i < (this.InternalChildren.Count - 1); i++)
			{
				var child = internalUiChildren[i];
				child.Arrange(new Rect(
					new Point(
						widthToDistribute / (this.InternalChildren.Count - 1) * i,
						heightToDistribute / (this.InternalChildren.Count - 1) * i),
					child.DesiredSize));
			}

			lastUiChild.Arrange(new Rect(
				new Point(widthToDistribute, heightToDistribute),
				lastUiChild.DesiredSize));

			return finalSize;

		}
	}
}
