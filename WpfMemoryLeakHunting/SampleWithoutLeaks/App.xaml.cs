using System.ComponentModel.Composition.Hosting;
using System.Reflection;
using System.Windows;

namespace WpfApplication19
{
	public partial class App : Application
	{
		public CompositionContainer Container { get; private set; }

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			// Setup DI container for executing assembly
			this.Container = new CompositionContainer(
				new AssemblyCatalog(Assembly.GetExecutingAssembly()));
		}
	}
}
