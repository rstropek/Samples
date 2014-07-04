// Note that this example CONTAINS MEMORY LEAKS. I use it in workshops to show how you can use
// memory profilers to find typical .NET memory leaks.

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
