using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Commands;
using MahApps.Metro.Controls;
using OpenCvSharp;
using MahApps.Metro.IconPacks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using TigerWord.Views;
using System.IO;
using System.Reflection;
using TigerWord.Core;
using OpenCvSharp.Dnn;
using TigerWord.Core.Services;
using System.Collections.ObjectModel;
using TigerWord.Core.Biz;
using TigerWord.Core.ViewModels;
using Prism.Events;
using System.Windows.Input;
using System.Xml.Linq;
using Unity;
using Wpf.Ui.Mvvm.Services;

namespace TigerWord.ViewModels
{
    public class MainMenuViewModel : BindableBase
    {
        public MainMenuViewModel(IUnityContainer container
            , IApplicationCommands applicationCommands
            , IEventAggregator ea)
        {
            if (MainWindow.current != null)
            {
                _dService = container.Resolve<IMahDialogService>();
                _dService.SetMahWindow(MainWindow.current);
            }
            ApplicationCommands = applicationCommands;
            // Menu Event Aggregator
            _ea = ea;
            // I Will Listen Menu Executed
            _ea.GetEvent<MenuSentEvent>().Subscribe(ListenMenuExecuted);
            // I Will Listen Menu Edited
            _ea.GetEvent<EditMenuEvent>().Subscribe(ListenMenumEdited);
            RunMenuCommand = new DelegateCommand<object>(RaiseExecuteMenu);

            //var b= CreateOCRMenu();
            CreateMenuItems();
        }

        #region Application Command
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }
        #endregion

        #region Property
        private int _menuIndex=-1;
        public int MenuIndex
        {
            get
            {
                return _menuIndex;
            }
            set
            {
                if (MenuDict.ContainsKey(value))
                {
                    ObservableCollection<CoreMenuData> menus = MenuDict[value];
                    RaiseEditMenu(menus);
                }
                SetProperty(ref _menuIndex, value);
            }
        }

        private HamburgerMenuItemCollection _menuItems;
        public HamburgerMenuItemCollection MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {

                SetProperty(ref _menuItems, value);
            }
        }


        private HamburgerMenuItemCollection _menuOptionItems;
        public HamburgerMenuItemCollection MenuOptionItems
        {
            get
            {
                return _menuOptionItems;
            }
            set
            {
                

                SetProperty(ref _menuOptionItems, value);
            }
        }
        #endregion
        private Dictionary<int, ObservableCollection<CoreMenuData>> MenuDict = new Dictionary<int, ObservableCollection<CoreMenuData>>();
            
        public void CreateMenuItems()
        {
            string current = Directory.GetCurrentDirectory();
            string[] fileEntries = Directory.GetFiles(current);
            MenuItems = new HamburgerMenuItemCollection();
            //MenuOptionItems = new HamburgerMenuItemCollection();

            // For DEBUG
            //this.MenuItems.Add(
            //    new HamburgerMenuIconItem()
            //    {
            //        Icon = new PackIconMaterial() { Kind = PackIconMaterialKind.AccountCircle },
            //        Label = "Private",
            //        ToolTip = "Private stuff.",
            //        Tag = new GoOCR.Views.TigerOCR()
            //    }
            //);
            MenuDict = new Dictionary<int, ObservableCollection<CoreMenuData>>();
            int index = 0;
            // For RELEASE
            foreach (string filename in fileEntries)
            {
                if (File.Exists(filename))
                {
                    if(filename.ToLower().Contains("tigerword.go") && filename.ToLower().EndsWith(".dll"))
                    {
                        Tuple<Type, string, string, PackIconBase, ObservableCollection<CoreMenuData>, object, object> moduleInfo =  SystemUtilityService.CreateModuleInfo(filename);
                        //object _moduleView = moduleInfo.Item1;
                        string _label = moduleInfo.Item2;
                        string _toolTip =   moduleInfo.Item3;
                        PackIconBase _icon = moduleInfo.Item4;
                        Type targetType = moduleInfo.Item1 as Type;
                        var _moduleView = Activator.CreateInstance(targetType);
                        MenuDict.Add(index++, moduleInfo.Item5);
                        //object _moduleView  = CreateModuleView(filename);
                        this.MenuItems.Add(
                            new HamburgerMenuIconItem()
                            {
                                Icon = _icon, //new PackIconMaterial() { Kind = PackIconMaterialKind.Home },
                                Label = _label,//"Home",
                                ToolTip = _toolTip,//"The Home view.",
                                Tag = _moduleView
                            }
                        );// ;

                    }
                }
            }
            if (index > 0)
                MenuIndex = 0;
        }

        #region Service
        private IEventAggregator _ea;
        private IMahDialogService _dService;
        private IMahDialogService DialogService
        {
            get { return _dService; }
            set { SetProperty(ref _dService, value); }
        }
        #endregion

        #region MenuReceive , MenuSend
        private void ListenMenuExecuted(object menu)
        {
            // Read type of menu // 
        }
        private void ListenMenumEdited(object menu)
        {

            // Read type of menu // 
        }

        public DelegateCommand<object> RunMenuCommand { get; set; }
        public DelegateCommand<object> EditMenuCommand { get; set; }

        private void RaiseExecuteMenu(object param)
        {
            _ea.GetEvent<MenuSentEvent>().Publish(param);
        }
        private void RaiseEditMenu(object param)
        {
            _ea.GetEvent<EditMenuEvent>().Publish(param);
        }

        #endregion
    }
}
