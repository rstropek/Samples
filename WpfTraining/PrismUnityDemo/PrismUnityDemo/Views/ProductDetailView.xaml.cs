using Microsoft.Practices.Unity;
using PrismUnityDemo.ViewModel;
using System;
using System.Windows.Controls;

namespace PrismUnityDemo.Views
{
    public partial class ProductDetailView : UserControl, IDisposable
	{
		private ProductDetailViewModel viewModel;

		public ProductDetailView(IUnityContainer container)
		{
			this.InitializeComponent();
            this.DataContext = this.viewModel = container.Resolve<ProductDetailViewModel>();
		}

        public void Dispose()
        {
            // TODO: Implement IDisposable correctly...

            if (this.viewModel != null)
            {
                this.viewModel.Dispose();
            }
        }
    }
}
