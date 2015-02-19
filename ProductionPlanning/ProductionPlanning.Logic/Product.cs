using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ProductionPlanning.Logic
{
	/// <summary>
	/// Implements a mutable product for data binding
	/// </summary>
	public class Product : BindableBase, ICompositeProduct
	{
		public Product()
		{ }

		public Product(Guid productID, string description,
			decimal costsPerItem, IEnumerable<IPart> parts = null)
		{
			this.ProductID = productID;
			this.Description = description;
			this.CostsPerItem = costsPerItem;
			if (parts != null)
			{
				foreach (var part in parts)
				{
					this.Parts.Add(part);
				}
			}
		}

		private Guid ProductIDValue;
		public Guid ProductID
		{
			get { return this.ProductIDValue; }
			set { this.SetProperty(ref this.ProductIDValue, value); }
		}

		private string DescriptionValue;
		public string Description
		{
			get { return this.DescriptionValue; }
			set { this.SetProperty(ref this.DescriptionValue, value); }
		}

		private decimal CostsPerItemValue;
		public decimal CostsPerItem
		{
			get { return this.CostsPerItemValue; }
			set { this.SetProperty(ref this.CostsPerItemValue, value); }
		}

		// Note read-only, auto-implemented property with initialization
		public ObservableCollection<IPart> Parts { get; } = new ObservableCollection<IPart>();

		// Note explicit interface implementation with function-bodied property
		IEnumerable<IPart> ICompositeProduct.Parts => this.Parts;
	}

	/// <summary>
	/// Implements a mutable part for data binding
	/// </summary>
	public class Part : BindableBase, IPart
	{
		private Guid ComponentProductIDValue;
		public Guid ComponentProductID
		{
			get { return this.ComponentProductIDValue; }
			set { this.SetProperty(ref this.ComponentProductIDValue, value); }
		}

		private int AmountValue;
		public int Amount
		{
			get { return this.AmountValue; }
			set { this.SetProperty(ref this.AmountValue, value); }
		}
	}
}
