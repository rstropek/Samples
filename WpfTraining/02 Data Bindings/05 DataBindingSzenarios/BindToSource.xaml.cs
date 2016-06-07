using System.Windows;

namespace Samples
{
    /// <summary>
    /// Interaction logic for BindToSource.xaml
    /// </summary>

    public partial class BindToSource : System.Windows.Controls.Page
	{
		public BindToSource()
		{
			InitializeComponent();
			ShowLastName.Click += new RoutedEventHandler(ShowLastName_Click);
		}

		void ShowLastName_Click(object sender, RoutedEventArgs e)
		{
			MessageBox.Show(((Person)FindResource("Person")).LastName);
		}

	}
}