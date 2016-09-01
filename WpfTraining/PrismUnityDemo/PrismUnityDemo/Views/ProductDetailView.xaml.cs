using Microsoft.Practices.Unity;
using PrismUnityDemo.ViewModel;
using System.Windows.Controls;

namespace PrismUnityDemo.Views
{
    public partial class ProductDetailView : UserControl
	{
		private ProductDetailViewModel viewModel;

		public ProductDetailView(IUnityContainer container)
		{
			this.InitializeComponent();
			this.DataContext = this.viewModel = container.Resolve<ProductDetailViewModel>();
		}
	}
}
