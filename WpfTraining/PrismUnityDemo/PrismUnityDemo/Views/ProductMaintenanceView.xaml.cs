using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using PrismUnityDemo.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace PrismUnityDemo.Views
{
	public partial class ProductMaintenanceView : UserControl
	{
		public ProductMaintenanceView()
		{
			InitializeComponent();
		}

        [InjectionMethod]
        public void OnInitialize(IUnityContainer container)
		{
            var vm = container.Resolve<ProductMaintenanceViewModel>();
			vm.NotificationService = msg => MessageBox.Show(msg);
			this.DataContext = vm;
		}
	}
}
