using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;
using TigerWord.Core.Services;

namespace TigerWord.Core
{
    public class Core : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RequestNavigate("ContentRegion", "PersonList");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }
}