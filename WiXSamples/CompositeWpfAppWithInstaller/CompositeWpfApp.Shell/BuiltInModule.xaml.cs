using CompositeWpfApp.Contract;
using System.ComponentModel.Composition;
using System.Windows.Controls;

namespace CompositeWpfApp.Shell
{
	public partial class BuiltInModule : UserControl, IModule
	{
		public BuiltInModule()
		{
			InitializeComponent();
		}
	}
}
