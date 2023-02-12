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

namespace TigerWord.GoPDF
{
    public class GoPDF: IModule
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
            return typeof(TigerWord.GoPDF.Views.TigerPDF);
        }

        public const string MenuText = "Tiger PDF";
        const string ToolTipText = "PDF Module";

        public Tuple<Type, string, string, MahApps.Metro.IconPacks.PackIconBase, ObservableCollection<CoreMenuData>,object,object> GetModuleInfo()
        {
            ObservableCollection<CoreMenuData> menu_info = GetMenuInfo();
            Tuple<Type, string, string, MahApps.Metro.IconPacks.PackIconBase, ObservableCollection<CoreMenuData>, object, object> retTup = new 
                Tuple<Type, string, string, MahApps.Metro.IconPacks.PackIconBase, ObservableCollection<CoreMenuData>
            , object, object>
                (
            typeof(TigerWord.GoPDF.Views.TigerPDF),
            MenuText,
            ToolTipText,
             new PackIconMaterial() { Kind = PackIconMaterialKind.Ocr },
             menu_info,
             null,
             null             
             );
            return retTup;
        }

        public static ObservableCollection<CoreMenuData> GetPdfMenuInfo()
        {
            ObservableCollection<CoreMenuData> ocrMenu = new ObservableCollection<CoreMenuData>();
            string ModuleID = MenuText;
            CoreMenuData FileMenu = new CoreMenuData("File", ModuleID, "file", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData OpenMenu = new CoreMenuData("Open...", ModuleID, "open", null, null, null);
                CoreMenuData ScanMenu = new CoreMenuData("Scan...", ModuleID, "open", null, null, null);
                CoreMenuData SaveMenu = new CoreMenuData("Save", ModuleID, "open", null, null, null);
                CoreMenuData SaveAsMenu = new CoreMenuData("Save As...", ModuleID, "open", null, null, null);
                
                CoreMenuData Div0 = new CoreMenuData("---", ModuleID, "div0", null, null, null);

                CoreMenuData RecentFilesMenu = new CoreMenuData("Recent Files", ModuleID, "open", null, null, null);
                {
                    ObservableCollection<CoreMenuData> recentFiles = new ObservableCollection<CoreMenuData>();
                    for(int i=0; i< 5; i++)
                    {
                        string submemuitem = string.Format("file_{0}", i);
                        CoreMenuData goOpenFile = new CoreMenuData("Long File Name ", ModuleID, submemuitem, null, null, null);
                        recentFiles.Append(goOpenFile);
                    }
                    RecentFilesMenu.MenuItems = recentFiles;
                }

                CoreMenuData Div1 = new CoreMenuData("---", ModuleID, "div1", null, null, null);

                SubMenuItems.Add(OpenMenu);
                SubMenuItems.Add(ScanMenu);
                SubMenuItems.Add(SaveMenu);
                SubMenuItems.Add(SaveAsMenu);
                SubMenuItems.Add(Div0);
                SubMenuItems.Add(RecentFilesMenu);
                SubMenuItems.Add(Div1);

                FileMenu.MenuItems = SubMenuItems;
            }
            CoreMenuData CommandMenu = new CoreMenuData("Command", ModuleID, "command", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData OcrMenu = new CoreMenuData("Ocr", ModuleID, "OcrMenu", null, null, null);
                CoreMenuData OcrAllPageMenu = new CoreMenuData("Ocr All Pages", ModuleID, "OcrAllPageMenu", null, null, null);

                CoreMenuData Div0 = new CoreMenuData("---", ModuleID, "Div0", null, null, null);

                CoreMenuData BulkMenu = new CoreMenuData("Bulk OCR...", ModuleID, "BulkMenu", null, null, null);

                CoreMenuData Div1 = new CoreMenuData("---", ModuleID, "div1", null, null, null);

                CoreMenuData PostProcessMenu = new CoreMenuData("Post-process", ModuleID, "PostProcessMenu", null, null, null);

                SubMenuItems.Add(OcrMenu);
                SubMenuItems.Add(OcrAllPageMenu);
                SubMenuItems.Add(Div0);
                SubMenuItems.Add(BulkMenu);
                SubMenuItems.Add(Div1);
                SubMenuItems.Add(PostProcessMenu);
                CommandMenu.MenuItems = SubMenuItems;
            }
            CoreMenuData ImageMenu = new CoreMenuData("Image", ModuleID, "image", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData Setting = new CoreMenuData("Cut", ModuleID, "setting", null, null, null);
                CoreMenuData Exit = new CoreMenuData("Paste", ModuleID, "exit", null, null, null);

                SubMenuItems.Add(Setting);
                SubMenuItems.Add(Exit);
                ImageMenu.MenuItems = SubMenuItems;
            }
            CoreMenuData FormatMenu = new CoreMenuData("Format", ModuleID, "format", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData Setting = new CoreMenuData("Cut", ModuleID, "setting", null, null, null);
                CoreMenuData Exit = new CoreMenuData("Paste", ModuleID, "exit", null, null, null);

                SubMenuItems.Add(Setting);
                SubMenuItems.Add(Exit);
                FormatMenu.MenuItems = SubMenuItems;
            }
            CoreMenuData SettingMenu = new CoreMenuData("Settings", ModuleID, "settings", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData Setting = new CoreMenuData("Cut", ModuleID, "setting", null, null, null);
                CoreMenuData Exit = new CoreMenuData("Paste", ModuleID, "exit", null, null, null);

                SubMenuItems.Add(Setting);
                SubMenuItems.Add(Exit);
                SettingMenu.MenuItems = SubMenuItems;
            }
            CoreMenuData ToolsMenu = new CoreMenuData("Tools", ModuleID, "tool", null, null, null);
            {
                ObservableCollection<CoreMenuData> SubMenuItems = new ObservableCollection<CoreMenuData>();
                CoreMenuData Setting = new CoreMenuData("Cut", ModuleID, "setting", null, null, null);
                CoreMenuData Exit = new CoreMenuData("Paste", ModuleID, "exit", null, null, null);

                SubMenuItems.Add(Setting);
                SubMenuItems.Add(Exit);
                ToolsMenu.MenuItems = SubMenuItems;
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
            ocrMenu.Add(CommandMenu);
            ocrMenu.Add(ImageMenu);
            ocrMenu.Add(FormatMenu);
            ocrMenu.Add(SettingMenu);
            ocrMenu.Add(ToolsMenu);
            ocrMenu.Add(ViewMenu);
            return ocrMenu;
        }

        public ObservableCollection<CoreMenuData> GetMenuInfo()
        {
            return Loader.GetPdfMenuInfo();            
        }
    }
}

