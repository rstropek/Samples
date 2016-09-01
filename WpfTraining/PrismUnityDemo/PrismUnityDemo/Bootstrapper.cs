using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Unity;
using PrismUnityDemo.Data;
using PrismUnityDemo.Modules;
using PrismUnityDemo.ViewModel;
using PrismUnityDemo.Contracts;
using PrismUnityDemo.Views;
using System.Windows;

namespace PrismUnityDemo
{
    class Bootstrapper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            this.Container.RegisterType<MainWindowNavigationController>(
                new ContainerControlledLifetimeManager());
            this.Container.RegisterType(typeof(ProductMaintenanceViewModel));
            this.Container.RegisterTypeForNavigation<ProductDetailView>(nameof(ProductDetailView));
            this.Container.RegisterType(typeof(ProductDetailViewModel));
            this.Container.RegisterType<GlobalCommands>(new ContainerControlledLifetimeManager());
        }

        protected override void ConfigureModuleCatalog()
        {
            this.ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = "Repository",
                ModuleType = typeof(RepositoryModule).AssemblyQualifiedName
            });
            this.ModuleCatalog.AddModule(new ModuleInfo
            {
                ModuleName = "Product Maintenance",
                ModuleType = typeof(ProductMaintenanceModule).AssemblyQualifiedName
            });
        }

        protected override void InitializeShell()
        {
            Application.Current.MainWindow.Show();
        }
    }
}
