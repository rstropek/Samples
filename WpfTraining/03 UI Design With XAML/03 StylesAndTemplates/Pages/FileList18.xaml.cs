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

namespace StylesAndTemplates.Pages
{
	/// <summary>
	/// Interaction logic for FileList18.xaml
	/// </summary>

	public partial class FileList18 : System.Windows.Controls.Page
	{
		public FileList18()
		{
			InitializeComponent();
		}

		private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Console.Out.WriteLine("Selected Item: " + ((ListBox)sender).SelectedItem.ToString());
		}
	}
}