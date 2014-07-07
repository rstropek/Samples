using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

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
				}
			}
		}

		#region INotifyPropertyChanged implementation
		private void RaisePropertyChanged([CallerMemberName]string propertyName = null)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		#endregion

		#region IDataErrorInfo implementation
		public string Error
		{
			get
			{
				var builder = new StringBuilder();
				Action<string> appendSubError = (subError) =>
				{
					if (!string.IsNullOrWhiteSpace(subError))
					{
						if (builder.Length > 0)
						{
							builder.Append('\n');
						}

						builder.Append(subError);
					}
				};

				appendSubError(this[() => this.CustomerName]);
				appendSubError(this[() => this.ProductName]);
				appendSubError(this[() => this.RebateCode]);
				appendSubError(this[() => this.OrderQuantity]);

				return builder.ToString();
			}
		}

		public string this[Expression<Func<object>> ex]
		{
			get
			{
				return this[Order.PropertyName(ex)];
			}
		}

		private static string PropertyName(Expression<Func<object>> ex)
		{
			var lambda = ex as LambdaExpression;
			if (lambda != null)
			{
				var memberAccess = lambda.Body as MemberExpression;
				if (memberAccess == null)
				{
					var unary = lambda.Body as UnaryExpression;
					if (unary != null)
					{
						memberAccess = unary.Operand as MemberExpression;
					}
				}

				if (memberAccess != null)
				{
					return memberAccess.Member.Name;
				}
			}

			return string.Empty;
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
				if (columnName == PropertyName(() => this.CustomerName)
					&& string.IsNullOrWhiteSpace(this.CustomerName))
				{
					return "Customer name is mandatory";
				}

				if (columnName == PropertyName(() => this.ProductName)
					&& string.IsNullOrWhiteSpace(this.ProductName))
				{
					return "Product name is mandatory";
				}

				if (columnName == PropertyName(() => this.OrderQuantity))
				{
					if (this.OrderQuantity <= 0)
					{
						return "Order quantity has to be greater than 0";
					}

					return checkRebateCode();
				}
				if (columnName == PropertyName(() => this.RebateCode))
				{
					return checkRebateCode();
				}

				return string.Empty;
			}
		}
		#endregion
	}
}
