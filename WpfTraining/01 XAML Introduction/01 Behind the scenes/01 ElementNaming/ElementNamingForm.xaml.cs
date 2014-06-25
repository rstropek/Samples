using System;
using System.Windows;

namespace ElementNaming
{
	public partial class ElementNamingForm : Window
	{
		public ElementNamingForm()
		{
			InitializeComponent();
		}

		private void OnCalculate_Click(Object sender, EventArgs e)
		{
			Result.Text = (Convert.ToInt32(Variable_X.Text) * Convert.ToInt32(Variable_Y.Text)).ToString();
		}
	}
}