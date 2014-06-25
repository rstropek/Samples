using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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