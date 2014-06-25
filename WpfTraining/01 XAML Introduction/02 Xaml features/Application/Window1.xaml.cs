using System;
using System.Windows.Controls;


namespace NamespaceSample
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>

	public partial class Window1 : System.Windows.Window
	{

		public Window1()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			lstPages.SelectionChanged += new SelectionChangedEventHandler(LstPages_SelectionChanged);
		}

		private void LstPages_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			frmContent.Source = new Uri("Pages/" + ((ListBoxItem)lstPages.SelectedValue).Content.ToString() + ".xaml", UriKind.Relative);
		}
	}
}