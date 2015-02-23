using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ProductionPlanning.Logic
{
	/// <summary>
	/// Implements a mutable product for data binding
	/// </summary>
	public class Product : BindableBase, ICompositeProduct, INotifyDataErrorInfo
	{
		// QUIZ: Would it be a good idea to turn Product into a struct?
		//       The answer is no. Why?

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
			set
			{
				var hasErrorsBeforeChange = this.HasErrors;
				if (this.SetProperty(ref this.CostsPerItemValue, value))
				{
					if (hasErrorsBeforeChange != this.HasErrors)
					{
						this.ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(nameof(this.CostsPerItem)));
					}
				}
			}
		}

		// Note read-only, auto-implemented property with initialization
		public ObservableCollection<IPart> Parts { get; } = new ObservableCollection<IPart>();

		// Note explicit interface implementation with function-bodied property
		// QUIZ: Do you know what happens with explicit interface implementations in the background?
		//       Check the resulting IL to find out.
		IEnumerable<IPart> ICompositeProduct.Parts => this.Parts;

		public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

		public bool HasErrors => this.CostsPerItem < 0;

		private static readonly string[] CostsPerItemMustNotBeNegative = new [] { "Costs per item must not be negative!" };
		private static readonly string[] NoErrors = new[] { string.Empty };

		public IEnumerable GetErrors(string propertyName) =>
			(string.IsNullOrEmpty(propertyName) || propertyName == nameof(CostsPerItem)) && this.HasErrors
				? CostsPerItemMustNotBeNegative : NoErrors;

		/*
		// QUIZ: Why would the following two overrides not be a good idea?
		//       Try to remove the comment and run the unit tests. One will fail.
		//       Try to find out why.

		public override bool Equals(object obj)
		{
			var p2 = obj as Product;
			if (p2 == null)
			{
				return false;
			}

			return this.ProductID == p2.ProductID 
				&& this.CostsPerItem == p2.CostsPerItem
				&& this.Description == p2.Description;
		}

		public override int GetHashCode()
		{
			return this.ProductID.GetHashCode() ^
				this.CostsPerItem.GetHashCode() ^
				this.Description.GetHashCode();
		}
		*/
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
