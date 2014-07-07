using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ValidationSample
{
	public class MaxValueValidationRule : ValidationRule
	{
		public int Maximum { get; set; }

		public override ValidationResult Validate(object value, CultureInfo cultureInfo)
		{
			var intValue = 0;
			if (value is Int32)
			{
				intValue = (int)value;
			}
			else if (!Int32.TryParse(value.ToString(), out intValue))
			{
				return new ValidationResult(false, "Could not convert value to integer");
			}

			if (intValue <= this.Maximum)
			{
				return new ValidationResult(true, null);
			}
			else
			{
				return new ValidationResult(false, "Value exceeds maximum");
			}
		}
	}

}
