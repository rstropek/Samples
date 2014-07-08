using System;

namespace BookshelfConfigurator.Data
{
	public static class ElementDimension
	{
		public static double HeightInCm(ElementHeight height)
		{
			switch (height)
			{
				case ElementHeight.Small:
					return 60.0;
				case ElementHeight.Medium:
					return 100.0;
				case ElementHeight.High:
					return 120.0;
				default:
					throw new NotImplementedException();
			}
		}

		public static double WidthInCm(ElementWidth width)
		{
			switch (width)
			{
				case ElementWidth.Narrow:
					return 60.0;
				case ElementWidth.Medium:
					return 100.0;
				case ElementWidth.Wide:
					return 120.0;
				default:
					throw new NotImplementedException();
			}
		}

		public static int MaximumNumberOfShelfs(ElementHeight height)
		{
			switch (height)
			{
				case ElementHeight.Small:
					return 1;
				case ElementHeight.Medium:
					return 3;
				case ElementHeight.High:
					return 6;
				default:
					throw new NotImplementedException();
			}
		}
	}
}
