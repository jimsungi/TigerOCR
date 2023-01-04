using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace TigerWord.GoOCR
{
    public class GoOCR: IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RequestNavigate("ContentRegion", "PersonList");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
