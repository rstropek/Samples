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
using System.Windows.Shapes;


namespace StylesAndTemplates
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class FileList : System.Windows.Window
	{
		public FileList()
		{
			InitializeComponent();
		}

		private void GoToPageExecuted(object target, ExecutedRoutedEventArgs e)
		{
			frmContent.Source = new Uri(e.Parameter.ToString(), UriKind.Relative);
		}

		private void GoToPageCanExecute(object sender, CanExecuteRoutedEventArgs e)
		{
			e.CanExecute = true;
		}
	}
}