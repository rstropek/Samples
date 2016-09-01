using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Regions;
using PrismUnityDemo.Contracts;
using PrismUnityDemo.ViewModel;
using PrismUnityDemo.Views;
using System;
using System.Linq;
using System.Windows;

namespace PrismUnityDemo.Views
{
    public class MainWindowNavigationController
	{
		private readonly IRegionManager regionManager;
        private readonly IUnityContainer container;

        public MainWindowNavigationController(IRegionManager regionManager, IUnityContainer container)
        {
            this.regionManager = regionManager;
            this.container = container;
        }

		public void ActivateProductDetailViewInjection(Product product)
		{
			var detailRegion = this.regionManager.Regions[RegionNames.DetailRegion];
			var existingView = detailRegion.Views
				.Cast<ProductDetailView>()
				.Where(p => p.DataContext == product)
				.FirstOrDefault();
			if (existingView != null)
			{
				detailRegion.Activate(existingView);
			}
			else
			{
                var pdv = this.container.Resolve<ProductDetailView>();
                ((ProductDetailViewModel)pdv.DataContext).Product = product;
				var rm = detailRegion.Add(
					pdv,
					string.Format("ProductDetailView{0}", product.ProductNumber),
					true);
				detailRegion.Activate(pdv);
			}
		}

		public void ActivateProductDetail(Product product)
		{
			var queryString = new NavigationParameters();
			queryString.Add("ProductNumber", product.ProductNumber.ToString());
			regionManager.RequestNavigate(
				RegionNames.DetailRegion,
				new Uri("ProductDetailView" + queryString.ToString(), UriKind.Relative));
		}

		public void RegisterDefaultSubscriptions(IEventAggregator eventAggregator)
		{
			eventAggregator
				.GetEvent<ProductSelectionChangedEvent>()
				.Subscribe(
					this.ActivateProductDetail,
					ThreadOption.UIThread,
					true);

			eventAggregator
				.GetEvent<CloseProductDetailEvent>()
				.Subscribe(
					this.CloseProductDetail,
					ThreadOption.UIThread,
					true);
		}

		public void CloseProductDetail(IProductDetailViewModel viewModel)
		{
			var detailRegion = this.regionManager.Regions[RegionNames.DetailRegion];
			var viewToClose = detailRegion.Views
				.Cast<FrameworkElement>()
				.Where(v => v.DataContext == viewModel)
				.FirstOrDefault();
			if (viewToClose != null)
			{
				var disposableViewToClose = viewToClose as IDisposable;
				if (disposableViewToClose != null)
				{
					disposableViewToClose.Dispose();
				}

				detailRegion.Remove(viewToClose);

				// TODO: Activate appropriate view
			}
		}
	}
}
