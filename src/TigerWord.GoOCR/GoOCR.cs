using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.IconPacks;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using TigerWord.Core;
using TigerWord.Core.Biz;

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

        public static string ModuleID
        {
            get { return Loader.MenuText; }
        }

    }

    public class Loader : TigerLoader
    {
        public Type GetModuleType()
        {
            return typeof(TigerWord.GoOCR.Views.TigerOCR);
        }

        public const string MenuText = "Tiger OCR";
        const string ToolTipText = "OCR Moudel";

        public Tuple<Type, string, string, MahApps.Metro.IconPacks.PackIconBase, ObservableCollection<CoreMenuData>,object,object> GetModuleInfo()
        {
            ObservableCollection<CoreMenuData> menu_info = GetMenuInfo();
            Tuple<Type, string, string, MahApps.Metro.IconPacks.PackIconBase, ObservableCollection<CoreMenuData>, object, object> retTup = new 
                Tuple<Type, string, string, MahApps.Metro.IconPacks.PackIconBase, ObservableCollection<CoreMenuData>
            , object, object>
                (
            typeof(TigerWord.GoOCR.Views.TigerOCR),
            MenuText,
            ToolTipText,
             new PackIconMaterial() { Kind = PackIconMaterialKind.Ocr },
             menu_info,
             null,
             null             
             );
            return retTup;
        }

        public static ObservableCollection<CoreMenuData> GetOcrMenuInfo()
        {
            ObservableCollection<CoreMenuData> ocrMenu = new ObservableCollection<CoreMenuData>();
            string ModuleID = MenuText;
            CoreMenuData FileMenu = new CoreMenuData("File", ModuleID, "file", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData Setting = new CoreMenuData("Setting", ModuleID, "settingx", null, null, null);
                CoreMenuData Exit = new CoreMenuData("Exit", ModuleID, "exitx", null, null, null);
                CoreMenuData Exit2 = new CoreMenuData("Exit2", ModuleID, "exitxx", null, null, null);

                SubMenuItems.Add(Setting);
                SubMenuItems.Add(Exit);
                SubMenuItems.Add(Exit2);
                FileMenu.MenuItems = SubMenuItems;
            }
            CoreMenuData EditMenu = new CoreMenuData("Edit", ModuleID, "edit", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData Setting = new CoreMenuData("Cut", ModuleID, "setting", null, null, null);
                CoreMenuData Exit = new CoreMenuData("Paste", ModuleID, "exit", null, null, null);

                SubMenuItems.Add(Setting);
                SubMenuItems.Add(Exit);
                EditMenu.MenuItems = SubMenuItems;
            }
            CoreMenuData ViewMenu = new CoreMenuData("View", ModuleID, "view", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData StandardView = new CoreMenuData("OCR", ModuleID, "standardview", null, null, null);
                CoreMenuData OCRView = new CoreMenuData("Step by Step Testing", ModuleID, "stepview", null, null, null);

                SubMenuItems.Add(StandardView);
                SubMenuItems.Add(OCRView);
                ViewMenu.MenuItems = SubMenuItems;
            }

            ocrMenu.Add(FileMenu);
            ocrMenu.Add(EditMenu);
            ocrMenu.Add(ViewMenu);
            return ocrMenu;
        }

        public ObservableCollection<CoreMenuData> GetMenuInfo()
        {
            return Loader.GetOcrMenuInfo();            
        }
    }
}

