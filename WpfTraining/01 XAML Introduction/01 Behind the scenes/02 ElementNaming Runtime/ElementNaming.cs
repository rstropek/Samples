using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Reflection;

namespace ElementNaming
{
	class ElementNaming : Application
	{
		[STAThread]
		private static void Main()
		{
			ElementNaming app = new ElementNaming();
			app.mainForm.Show();
			app.Run();
		}

		private Window mainForm;
		private TextBox Variable_X;
		private TextBox Variable_Y;
		private TextBox Result;
		private Button Calculate;

		public ElementNaming()
		{
			string location = Assembly.GetExecutingAssembly().Location;
			using (var fs = new FileStream(
				Path.Combine(Path.GetDirectoryName(location), Path.GetFileNameWithoutExtension(location) + "Form.xaml"),
				FileMode.Open))
			{
				mainForm = (Window)XamlReader.Load(fs);
				DoWiring(mainForm);
			}
		}

		private void DoWiring(DependencyObject formContent)
		{
			Variable_X = (TextBox)LogicalTreeHelper.FindLogicalNode(formContent, "Variable_X");
			Variable_Y = (TextBox)LogicalTreeHelper.FindLogicalNode(formContent, "Variable_Y");
			Result = (TextBox)LogicalTreeHelper.FindLogicalNode(formContent, "Result");
			Calculate = (Button)LogicalTreeHelper.FindLogicalNode(formContent, "Calculate");
			Calculate.Click += new RoutedEventHandler(OnCalculate_Click);
		}

		private void OnCalculate_Click(Object sender, EventArgs e)
		{
			Result.Text = (Convert.ToInt32(Variable_X.Text) * Convert.ToInt32(Variable_Y.Text)).ToString();
		}
	}
}
