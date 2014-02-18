using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AsyncSensors
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();

			var vm = this.DataContext as MainWindowViewModel;
			vm.ConfirmationHandler = () =>
				{
					// Note the use of the TaskCompletionSource here
					var tcs = new TaskCompletionSource<bool>();

					// Dynamically build confirmation dialog (just for demo purposes; you
					// could use XAML, too).
					#region Confirmation dialog
					var dialog = new Window() { SizeToContent = SizeToContent.WidthAndHeight };
					var stackPanel = new StackPanel() { Margin = new Thickness(10.0) };
					dialog.Content = stackPanel;

					stackPanel.Children.Add(new TextBlock() { Text = "Are you sure?" });
					Button yesButton, noButton;
					stackPanel.Children.Add(yesButton = new Button() {
						Content = "Yes"
					});
					yesButton.Click += (_, __) => {
						tcs.SetResult(true);
						dialog.Hide();
					};
					stackPanel.Children.Add(noButton = new Button()
					{
						Content = "No"
					});
					noButton.Click += (_, __) =>
					{
						tcs.SetResult(false);
						dialog.Hide();
					};
					#endregion

					dialog.Show();

					return tcs.Task;
				};
		}
	}
}
