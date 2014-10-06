using System.Windows;

namespace CompositeWpfApp.Shell
{
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var vm = new MainWindowViewModel();
			vm.RefreshModuleList();
			this.DataContext = vm;
		}
	}
}
