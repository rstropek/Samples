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

            // Singletons -> use ContainerControlledLifetimeManager
            this.Container.RegisterType<MainWindowNavigationController>(new ContainerControlledLifetimeManager());
            this.Container.RegisterType<GlobalCommands>(new ContainerControlledLifetimeManager());

            // Register view that is used for Prism navigation
            this.Container.RegisterTypeForNavigation<ProductDetailView>(nameof(ProductDetailView));

            // Register view models
            this.Container.RegisterType<ProductMaintenanceViewModel>();
            this.Container.RegisterType<ProductDetailViewModel>();
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
