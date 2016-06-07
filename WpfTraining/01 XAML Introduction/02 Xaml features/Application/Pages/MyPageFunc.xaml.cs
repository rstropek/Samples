using System;
using System.Windows;
using System.Windows.Navigation;

namespace NamespaceSample.Pages
{
	public partial class MyPageFunc : System.Windows.Navigation.PageFunction<String>
	{

		public MyPageFunc()
		{
			InitializeComponent();
		}

		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			btnReturn.Click += BtnReturn_Click;
		}

		private void BtnReturn_Click(object sender, RoutedEventArgs e)
		{
			this.OnReturn(new ReturnEventArgs<string>(txtReturn.Text));
		}
	}
}