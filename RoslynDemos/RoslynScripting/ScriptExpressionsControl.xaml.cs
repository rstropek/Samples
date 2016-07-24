using System.Windows.Controls;

namespace RoslynScripting
{
	public partial class ScriptExpressionsControl : UserControl
	{
		public ScriptExpressionsControl()
		{
			InitializeComponent();
			this.DataContext = "Hello World!";
		}
	}
}
