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
using System.Windows.Markup;


namespace CombinedGeometry
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{

		public Window1()
		{
			InitializeComponent();
			ExportGeometry.Click += new RoutedEventHandler(ExportGeometry_Click);
		}

		void ExportGeometry_Click(object sender, RoutedEventArgs e)
		{
			string catEyeXaml = XamlWriter.Save(CatEye);
			MessageBox.Show(catEyeXaml);
		}
	}
}