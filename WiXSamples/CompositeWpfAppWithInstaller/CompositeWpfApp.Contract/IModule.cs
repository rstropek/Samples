using System.ComponentModel.Composition;

namespace CompositeWpfApp.Contract
{
	/// <summary>
	/// Modules have to implement this interface
	/// </summary>
	[InheritedExport("Module", typeof(IModule))]
	public interface IModule
    {
    }
}
