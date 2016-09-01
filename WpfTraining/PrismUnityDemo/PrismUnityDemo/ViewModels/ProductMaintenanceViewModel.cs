using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using PrismUnityDemo.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace PrismUnityDemo.ViewModel
{
    public class ProductMaintenanceViewModel : BindableBase
	{
		private readonly Repository repository = null;
		private readonly DelegateCommand refreshProductListCommand = null;

		//[Import]
		//private NavigationController navigationController = null;

		public ProductMaintenanceViewModel(Repository repository, IEventAggregator eventAggregator)
		{
            this.repository = repository;
            this.eventAggregator = eventAggregator;

			this.ProductNumberFilter = 0;

			this.refreshProductListCommand = new DelegateCommand(
				this.RefreshProductList);
		}

		public Action<string> NotificationService { get; set; }

		private Product SelectedProductValue;
		public Product SelectedProduct
		{
			get
			{
				return this.SelectedProductValue;
			}

			set
			{
				if (this.SelectedProductValue != value)
				{
					this.SelectedProductValue = value;
					this.OnPropertyChanged(() => this.SelectedProduct);
					//this.navigationController.ActivateProductDetail(this.SelectedProduct);

					this.eventAggregator
						.GetEvent<ProductSelectionChangedEvent>()
						.Publish(this.SelectedProduct);
				}
			}
		}

		private IEventAggregator eventAggregator = null;

		private IEnumerable<Product> ProductsValue;
		public IEnumerable<Product> Products
		{
			get
			{
				return this.ProductsValue;
			}

			set
			{
				if (this.ProductsValue != value)
				{
					this.ProductsValue = value;
					this.OnPropertyChanged(() => this.Products);
				}
			}
		}

		private int ProductNumberFilterValue;
		public int ProductNumberFilter
		{
			get
			{
				return this.ProductNumberFilterValue;
			}

			set
			{
				if (this.ProductNumberFilterValue != value)
				{
					this.ProductNumberFilterValue = value;
					this.OnPropertyChanged(() => this.ProductNumberFilter);
				}
			}
		}

		private string ProductNameFilterValue;
		public string ProductNameFilter
		{
			get
			{
				return this.ProductNameFilterValue;
			}

			set
			{
				if (this.ProductNameFilterValue != value)
				{
					this.ProductNameFilterValue = value;
					this.OnPropertyChanged(() => this.ProductNameFilter);
				}
			}
		}

		public ICommand RefreshProductListCommand
		{
			get
			{
				return this.refreshProductListCommand;
			}
		}

		public void RefreshProductList()
		{
			var query = this.repository.SelectAllProducts();

			if (this.ProductNumberFilter != 0)
			{
				query = query.Where(p => p.ProductNumber == this.ProductNumberFilter);
			}

			if (!string.IsNullOrWhiteSpace(this.ProductNameFilter))
			{
				query = query.Where(p => p.ProductName.Contains(this.ProductNameFilter));
			}

			var counter = query.Count();
			if (counter > 5 && this.NotificationService != null)
			{
				this.NotificationService("Too many results");
			}
			else
			{
				this.Products = query.ToArray();
			}
		}
	}
}
