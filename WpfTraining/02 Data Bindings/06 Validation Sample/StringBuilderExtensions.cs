using System.Text;
using System.Windows;

namespace ValidationSample
{
	public static class StringBuilderExtensions
	{
		public static void AppendSeparatedIfNotEmpty(this StringBuilder builder, char separation, object value)
		{
			if (value != DependencyProperty.UnsetValue)
			{
				var valueAsString = value.ToString();
				if (!string.IsNullOrWhiteSpace(valueAsString))
				{
					if (builder.Length > 0)
					{
						builder.Append(separation);
					}

					builder.Append(valueAsString);
				}
			}
		}
	}
}
