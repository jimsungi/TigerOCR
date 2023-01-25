using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using TigerWord.Core.Services;
using MahApps.Metro.IconPacks;
using System.Collections.ObjectModel;
using TigerWord.Core.Biz;

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

    public interface TigerLoader
    {
        public Type GetModuleType();
        public Tuple<Type, string, string, MahApps.Metro.IconPacks.PackIconBase, ObservableCollection<CoreMenuData>, object, object>  GetModuleInfo();
    }
}