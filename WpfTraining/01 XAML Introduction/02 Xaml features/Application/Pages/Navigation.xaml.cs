using System;
using System.Windows;
using System.Windows.Navigation;

namespace NamespaceSample.Pages
{
	public partial class Navigation : System.Windows.Controls.Page
	{
		public Navigation()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			btnNavigate.Click += new RoutedEventHandler(BtnNavigate_Click);
		}

		private void BtnNavigate_Click(object sender, RoutedEventArgs e)
		{
			PageFunction<string> pageFunction = new MyPageFunc();
			pageFunction.Return += new ReturnEventHandler<string>(PageFunction_Return);
			this.NavigationService.Navigate(pageFunction);
		}

		private void PageFunction_Return(object sender, ReturnEventArgs<string> e)
		{
			MessageBox.Show(e.Result);
		}
	}
}