using IronPython.UI.ViewModel;
using Microsoft.Windows.Controls.Ribbon;

namespace IronPython.UI
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow
	{
		private MainWindowViewModel viewModel;

		public MainWindow()
		{
			this.InitializeComponent();
			this.Loaded += (s, e) => { this.DataContext = this.viewModel = new MainWindowViewModel(); };
		}
	}
}
