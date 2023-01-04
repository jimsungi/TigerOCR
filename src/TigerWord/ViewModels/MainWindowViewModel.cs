using Prism.Mvvm;
using System.Collections.Generic;
using TigerWord.Views;
using Unity;
using TigerWord.Core.Services;
using TigerWord.Core.ViewModels;
using Prism.Commands;


namespace TigerWord.ViewModels
{



    public class MainWindowViewModel : BindableBase
    {
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
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }
        private IMahThemeService _themeService;
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
        public MainWindowViewModel(IUnityContainer container
            ,IMahThemeService themeService
            ,IApplicationCommands applicationCommands
            ,IUserSettingService setService)
        {
            if(MainWindow.current !=null)
            {
                IMahDialogService dService =  container.Resolve<IMahDialogService>();
                dService.SetMahWindow(MainWindow.current);
            }
            ThemeService = themeService;
            ApplicationCommands = applicationCommands;
            UserSettingService = setService;
            ThemeService.AppCmd = ApplicationCommands;
            ThemeService.Setting = UserSettingService;

            ApplicationCommands.ThemeMenuReloadCommand.RegisterCommand(ReloadThemeCmd);
            ApplicationCommands.ThemeAccentReloadCommand.RegisterCommand(ReloadAccentCmd);

            string theme = UserSettingService.GetSetting("theme");
            string accent = UserSettingService.GetSetting("accent");
            if(!string.IsNullOrWhiteSpace(theme))
            {
                ThemeService.SetTheme(theme);
            }
            if(!string.IsNullOrWhiteSpace(accent))
            {
                ThemeService.SetAccent(accent);
            }

            this.AccentColors = ThemeService.GetAccentContextData();
            this.AppThemes = ThemeService.GetThemeContextData();
            
        }

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

    }
}
