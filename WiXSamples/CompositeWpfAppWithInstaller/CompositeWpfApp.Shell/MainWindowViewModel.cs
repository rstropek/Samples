using CompositeWpfApp.Contract;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace CompositeWpfApp.Shell
{
	public class MainWindowViewModel
	{
		private ObservableCollection<object> ModulesValue = new ObservableCollection<object>();

		public void RefreshModuleList()
		{
			this.ModulesValue.Clear();

			using (var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
			using (var directoryCatalog = new DirectoryCatalog(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Modules")))
			using (var aggregateCatalog = new AggregateCatalog(assemblyCatalog, directoryCatalog))
			using (var container = new CompositionContainer(aggregateCatalog))
			{
				foreach(var module in container.GetExportedValues<IModule>("Module"))
				{
					this.ModulesValue.Add(module);
				}
			}
		}

		public IEnumerable Modules { get { return this.ModulesValue; } }
	}
}
