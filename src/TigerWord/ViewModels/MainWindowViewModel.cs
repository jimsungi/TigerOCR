using Prism.Mvvm;
using System.Collections.Generic;
using TigerWord.Views;
using Unity;
using TigerWord.Core.Services;
using TigerWord.Core.ViewModels;
using Prism.Commands;
using TigerWord.Core.Biz;
using System.Collections.ObjectModel;
using Prism.Events;
using System.Linq;
using System.Windows;
using System;
using System.Printing;
using Unity.Injection;
using System.Windows.Controls;

namespace TigerWord.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        public MainWindowViewModel(IUnityContainer container
            , IMahThemeService themeService
            , IApplicationCommands applicationCommands
            , IUserSettingService setService
            , IEventAggregator ea)
        {
            // Dialog Servie setting
            if (MainWindow.current != null)
            {
                _dService = container.Resolve<IMahDialogService>();
                _dService.SetMahWindow(MainWindow.current);
            }
            // Initialize Service and Command
            ThemeService = themeService;
            ApplicationCommands = applicationCommands;
            UserSettingService = setService;
            ThemeService.AppCmd = ApplicationCommands;
            ThemeService.Setting = UserSettingService;

            // Application Commands
            ApplicationCommands.ThemeMenuReloadCommand.RegisterCommand(ReloadThemeCmd);
            ApplicationCommands.ThemeAccentReloadCommand.RegisterCommand(ReloadAccentCmd);

            // Menu Event Aggregator
            CoreMenuData.EA = _ea = ea;
            _ea.GetEvent<MenuSentEvent>().Subscribe(ListenMenuExecuted);
            _ea.GetEvent<EditMenuEvent>().Subscribe(ListenMenuEdited);
            RunMenuCommand = new DelegateCommand<object>(RaiseMenuExecute);

            // Load personal theme setting
            string theme = UserSettingService.GetSetting("theme");
            string accent = UserSettingService.GetSetting("accent");
            if (!string.IsNullOrWhiteSpace(theme))
            {
                ThemeService.SetTheme(theme);
            }
            if (!string.IsNullOrWhiteSpace(accent))
            {
                ThemeService.SetAccent(accent);
            }

            this.AccentColors = ThemeService.GetAccentContextData();
            this.AppThemes = ThemeService.GetThemeContextData();

            // Initialize Menu
            this.LeftMenu = InitMenu();
        }

        #region Property

        private ObservableCollection<CoreMenuData> _leftMenu;
        public ObservableCollection<CoreMenuData> LeftMenu
        {
            get
            {
                return _leftMenu;
            }
            set
            {
                SetProperty(ref _leftMenu, value);
            }
        }

        private List<AccentColorMenuData> _AccentColors;
        public List<AccentColorMenuData> AccentColors
        {
            get { return _AccentColors; }
            set
            {
                _AccentColors = value;
                RaisePropertyChanged("AccentColors");
            }
        }

        private List<AppThemeMenuData> _AppThemes;
        public List<AppThemeMenuData> AppThemes
        {
            get { return _AppThemes; }
            set
            {
                _AppThemes = value;
                RaisePropertyChanged("AppThemes");
            }
        }
        #endregion Property

        #region Application Command
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }
        #endregion

        #region DelegageCommand

        private DelegateCommand _reloadThemeMenu;
        private DelegateCommand _reloadAccentMenu;
        public DelegateCommand ReloadThemeCmd =>
            _reloadThemeMenu ?? (_reloadThemeMenu = new DelegateCommand(ReloadThemeFunc));
        public DelegateCommand ReloadAccentCmd =>
            _reloadAccentMenu ?? (_reloadAccentMenu = new DelegateCommand(ReloadAccentFunc));

        void ReloadThemeFunc()
        {
            this.AppThemes = ThemeService.GetThemeContextData();
        }
        void ReloadAccentFunc()
        {
            this.AccentColors = ThemeService.GetAccentContextData();
        }
        #endregion

        #region Service
        private IEventAggregator _ea;
        private IMahThemeService _themeService;
        private IMahDialogService _dService;
        private IMahDialogService DialogService
        {
            get { return _dService; }
            set { SetProperty(ref _dService, value); }
        }

        public IMahThemeService ThemeService
        {
            get { return _themeService; }
            set { SetProperty(ref _themeService, value); }
        }

        private IUserSettingService _UserSettingService;
        public IUserSettingService UserSettingService
        {
            get { return _UserSettingService; }
            set { SetProperty(ref _UserSettingService, value); }
        }
        #endregion

        #region Main WIndow System Menu
        static string ModuleID = "Application";
        
        // cmd by menu      
        private DelegateCommand _exitCmd;
        public DelegateCommand ExitCmd =>
            _exitCmd ?? (_exitCmd = new DelegateCommand(ExitFunc));
        void ExitFunc()
        {
            Application.Current.Shutdown(0);
        }


        private DelegateCommand _settingCmd;
        public DelegateCommand SettingCmd =>
            _settingCmd ?? (_settingCmd = new DelegateCommand(SettingFunc));
        void SettingFunc()
        {
            DialogService.ShowMessage("setting not implemented", "");
            // throw new NotImplementException();
        }


        /// <summary>
        /// As Default MainMenu is set
        /// When u provide a App, aka when u load a App, App Menu is inserted in  MainMenu
        /// App's menu location is as dash - replaced.
        /// 
        /// root_menu_order is a root menu or top menu.  Apps top menu is replaced in "-" location.(aka a menu not listed in root_menu_order placed in "-"
        /// 
        /// if App top menu has file, edit, tool, window, help
        /// its followed by file_menu_order, etc, using menu_order Dictionay. Position is with "-"
        /// </summary>
        ObservableCollection<CoreMenuData> MainMenu = new ObservableCollection<CoreMenuData>();
        static string[] root_menu_order = { "file", "edit", "-", "tool", "window", "help" };
        static string[] file_menu_order = { "-", "SETTING", "EXIT" };
        static string[] edit_menu_order = { "-" };
        static string[] tool_menu_order = { "-" };
        static string[] window_menu_order = { "-" };
        static string[] help_menu_order = { "-" };

        Dictionary<string, string[]> menu_order = new Dictionary<string, string[]>
        {
            { "-", root_menu_order },
            { "file", file_menu_order },
            { "edit", edit_menu_order },
            { "tool", tool_menu_order },
            { "window", window_menu_order },
            { "help", help_menu_order },
        };

        ObservableCollection<CoreMenuData> InitMenu()
        {
            CoreMenuData FileMenu = new CoreMenuData("File", ModuleID, "file", null, null, null);
            ObservableCollection<CoreMenuData> FileMenuItems = new ObservableCollection<CoreMenuData>();
            {
                CoreMenuData Setting = new CoreMenuData("Setting", ModuleID, "SETTING", null, null, null);
                CoreMenuData Exit = new CoreMenuData("Exit", ModuleID, "EXIT", null, null, null);

                FileMenuItems.Add(Setting);
                FileMenuItems.Add(Exit);
                FileMenu.MenuItems = FileMenuItems;
            }

            MainMenu.Add(FileMenu);
            return MainMenu;
        }


        #endregion Menu

        #region MenuReceive , MenuSend
        private void ListenMenuExecuted(object menu)
        {
            string menu_txt = menu as string;
            string[] menu_path = menu_txt.Split(":");
            if(menu_path !=null && menu_path.Length == 2)
            {
                if (menu_path[0] == ModuleID)
                {
                    switch(menu_path[1])
                    {
                        case "EXIT":
                            ExitFunc();
                            break;
                        case "SETTING":
                            SettingFunc();
                            break;
                    }
                }
            }
            // Read type of menu // 
        }
        private void ListenMenuEdited(object menu)
        {
            ObservableCollection<CoreMenuData> editingMenu = menu as ObservableCollection<CoreMenuData>;
            if(editingMenu !=null)
            {
                ObservableCollection<CoreMenuData> new_menu = SystemUtilityService.ReorderMenu(menu_order, ModuleID, MainMenu, editingMenu);
                this.LeftMenu = new_menu;
            }
            // Read type of menu // 
        }

        public DelegateCommand<object> RunMenuCommand { get; set; }

        private void RaiseMenuExecute(object param)
        {
            _ea.GetEvent<MenuSentEvent>().Publish(param);
        }

        #endregion
    }
}
