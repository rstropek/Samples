using Prism.Modularity;
using Prism.Regions;
using PrismUnityDemo.Views;

namespace PrismUnityDemo.Modules
{
    public class ProductMaintenanceModule : IModule
    {
        private readonly IRegionManager regionManager;

        public ProductMaintenanceModule(IRegionManager regionManager)
        {
            this.regionManager = regionManager;
        }

        public void Initialize()
        {
            regionManager.RegisterViewWithRegion(RegionNames.MainContentRegion, 
                typeof(ProductMaintenanceView));
        }
    }
}
