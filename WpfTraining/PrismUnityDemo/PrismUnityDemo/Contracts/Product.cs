using System.ComponentModel;

namespace PrismUnityDemo.Contracts
{
	public class Product : IDataErrorInfo
	{
		public int ProductNumber { get; set; }
		public string ProductName { get; set; }

		public string Error
		{
			get 
			{
				return this["ProductNumber"] + this["ProductName"];
			}
		}

		public string this[string columnName]
		{
			get 
			{
				switch (columnName)
				{
					case "ProductNumber":
						if (this.ProductNumber < 0)
						{
							return "Product number must not be negative.";
						}
						break;
					default:
						break;
				}

				return string.Empty;
			}
		}
	}
}
