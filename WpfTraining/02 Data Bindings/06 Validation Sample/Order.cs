using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

namespace ValidationSample
{
    public class Order : INotifyPropertyChanged, IDataErrorInfo
	{
		private string CustomerNameValue;
		public string CustomerName
		{
			get { return this.CustomerNameValue; }
			set
			{
				if (this.CustomerNameValue != value)
				{
					this.CustomerNameValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private string ProductNameValue;
		public string ProductName
		{
			get { return this.ProductNameValue; }
			set
			{
				if (this.ProductNameValue != value)
				{
					this.ProductNameValue = value;
					this.RaisePropertyChanged();
				}
			}
		}

		private string RebateCodeValue;
		public string RebateCode
		{
			get { return this.RebateCodeValue; }
			set
			{
				if (this.RebateCodeValue != value)
				{
					this.RebateCodeValue = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.OrderQuantity));
				}
			}
		}

		private int OrderQuantityValue;
		public int OrderQuantity
		{
			get { return this.OrderQuantityValue; }
			set
			{
				if (this.OrderQuantityValue != value)
				{
					this.OrderQuantityValue = value;
					this.RaisePropertyChanged();
					this.RaisePropertyChanged(nameof(this.RebateCode));
				}
			}
		}

		#region INotifyPropertyChanged implementation
		private void RaisePropertyChanged([CallerMemberName]string propertyName = null) =>
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region IDataErrorInfo implementation
		public string Error
		{
			get
			{
				return new[] {
					this[nameof(CustomerName)], 
					this[nameof(ProductName)],
					this[nameof(RebateCode)], 
					this[nameof(OrderQuantity)]
				}
				.Distinct()
				.Aggregate<string, StringBuilder, string>(
					new StringBuilder(),
					(builder, next) => { builder.AppendSeparatedIfNotEmpty('\n', next); return builder; },
					builder => builder.ToString());
			}
		}

		public string this[string columnName]
		{
			get
			{
				Func<string> checkRebateCode = () =>
				{
					if (!string.IsNullOrWhiteSpace(this.RebateCode) && this.OrderQuantity > 1)
					{
						return "You can only order one item if you use a rebate code";
					}

					return string.Empty;
				};
				if (columnName == nameof(this.CustomerName)
					&& string.IsNullOrWhiteSpace(this.CustomerName))
				{
					return "Customer name is mandatory";
				}

				if (columnName == nameof(this.ProductName)
					&& string.IsNullOrWhiteSpace(this.ProductName))
				{
					return "Product name is mandatory";
				}

				if (columnName == nameof(this.OrderQuantity))
				{
					if (this.OrderQuantity <= 0)
					{
						return "Order quantity has to be greater than 0";
					}

					return checkRebateCode();
				}

				if (columnName == nameof(this.RebateCode))
				{
					return checkRebateCode();
				}

				return string.Empty;
			}
		}
		#endregion
	}
}
