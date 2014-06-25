using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Samples
{
	public partial class BindingExpressions : Page
	{
		public BindingExpressions()
		{
			InitializeComponent();
			WriteDataToSource.Click += new RoutedEventHandler(WriteDataToSource_Click);

			Binding myBinding = new Binding("LastName");
			myBinding.Source = FindResource("Person");
			myBinding.UpdateSourceTrigger = UpdateSourceTrigger.LostFocus;
			LostFocusBox.SetBinding(TextBox.TextProperty, myBinding);
			LostFocusTextBlock.SetBinding(TextBlock.TextProperty, myBinding);

			BindingExpression textBoxExpression = 
				LostFocusBox.GetBindingExpression(TextBox.TextProperty);
			BindingExpression textBlockExpression = 
				LostFocusTextBlock.GetBindingExpression(TextBlock.TextProperty);
		}

		void WriteDataToSource_Click(object sender, RoutedEventArgs e)
		{
			BindingExpression expr = ExplicitBox.GetBindingExpression(TextBox.TextProperty);
			expr.UpdateSource();
		}

	}
}