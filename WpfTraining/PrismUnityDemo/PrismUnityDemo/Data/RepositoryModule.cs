using Microsoft.Practices.Unity;
using Prism.Modularity;
using PrismUnityDemo.Contracts;
using PrismUnityDemo.Data;

namespace PrismUnityDemo.Data
{
    public class RepositoryModule : IModule
	{
        private IUnityContainer container;

        public RepositoryModule(IUnityContainer container)
        {
            this.container = container;   
        }

		public void Initialize()
		{
			// Add some code to configure appropriate repository implementation
			var memRepo = new MemoryRepository();

            this.container.RegisterInstance<IRepository>(new MemoryRepository());
		}
	}
}
