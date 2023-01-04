using ControlzEx.Theming;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using TigerWord.Core.ViewModels;
using Unity;

namespace TigerWord.Core.Services
{

    public class AccentColorMenuData 
    {
        public string Name { get; set; }

        public Brush IconBorderColorBrush { get; set; }

        public Brush IconColorBrush { get; set; }
        public bool IsChecked { get; set; }
        public bool IsCheckable { get; set; }
        public IMahThemeService themeService = null;

        public AccentColorMenuData(IMahThemeService _themeService)
        {
            themeService = _themeService;
            this.ChangeAccentCommand = new SimpleCommand(o => true, this.DoChangeTheme);
        }

        public ICommand ChangeAccentCommand { get; }

        protected virtual void DoChangeTheme(object sender)
        {
            //if (IsChecked)
            //{
         
            //}
            //else
            //{
                themeService.SetAccent(this.Name);
                themeService.ReloadAccentMenu();
            //}
        }
    }
    public class AppThemeMenuData : AccentColorMenuData
    {
        public AppThemeMenuData(IMahThemeService _themeService) : base(_themeService)
        {
        }
        protected override void DoChangeTheme(object sender)
        {
            //if (IsChecked)
            //{

            //}
            //else
            //{
                themeService.SetTheme(this.Name);
                themeService.ReloadThemeMenu();
            //}
        }
    }

    public interface IMahThemeService
    {
        IApplicationCommands AppCmd { get; set; }
        IUserSettingService Setting { get; set; }

        string Name { get; }
        void SetTheme(string name);
        string GetTheme();
        void SetAccent(string name);
        string GetAccent();
        string GetName();
        List<AppThemeMenuData> GetThemeContextData();
        List<AccentColorMenuData> GetAccentContextData();
        void ReloadThemeMenu();
        void ReloadAccentMenu();
    }
    public class MahThemeService : IMahThemeService
    {
        public IApplicationCommands AppCmd { get; set; }
        public IUserSettingService Setting { get; set; }

        public void ReloadThemeMenu()
        {
            AppCmd.ThemeMenuReloadCommand.Execute("");
        }

        public void ReloadAccentMenu()
        {
            AppCmd.ThemeAccentReloadCommand.Execute("");
        }

        public string Name => GetName();

        public string GetName()
        {
            Theme? cTheme = ThemeManager.Current.DetectTheme();
            if (cTheme != null)
            {
                return cTheme.Name;
            }
            return "";
        }


        public string GetAccent()
        {
            Theme? cTheme = ThemeManager.Current.DetectTheme();
            if (cTheme != null)
            {
                return cTheme.ColorScheme;
            }
            return "";
        }

        public string GetTheme()
        {
            Theme? cTheme = ThemeManager.Current.DetectTheme();
            if (cTheme != null)
            {
                return cTheme.BaseColorScheme;
            }
            return "";
        }

        public void SetAccent(string name)
        {
            ThemeManager.Current.ChangeThemeColorScheme(Application.Current, name);
            Setting.SetSetting("accent", name);
        }

        public void SetTheme(string name)
        {
            ThemeManager.Current.ChangeThemeBaseColor(Application.Current, name);
            Setting.SetSetting("theme", name);
        }
        public List<AccentColorMenuData> GetAccentContextData()
        {
            string accent = GetAccent();
            var acccc = ThemeManager.Current.Themes
                               .GroupBy(x => x.ColorScheme)
                               .OrderBy(a => a.Key)
                               .Select(a => new AccentColorMenuData(this) { Name = a.Key, IconColorBrush = a.First().ShowcaseBrush })
                               .ToList();
            foreach (var each in acccc)
            {
                if (accent == each.Name)
                {
                    each.IsChecked = true;
                }
                else
                {
                    each.IsChecked = false;
                }
                each.IsCheckable = true;
            }
            return acccc;
        }
        public List<AppThemeMenuData> GetThemeContextData()
        {
            string theme = GetTheme();
            var appth = ThemeManager.Current.Themes
                                         .GroupBy(x => x.BaseColorScheme)
                                         .Select(x => x.First())
                                         .Select(a => new AppThemeMenuData(this) { Name = a.BaseColorScheme, IconBorderColorBrush = a.Resources["MahApps.Brushes.ThemeForeground"] as Brush, IconColorBrush = a.Resources["MahApps.Brushes.ThemeBackground"] as Brush })
                                         .ToList();
            foreach (var each in appth)
            {
                if (theme == each.Name)
                {
                    each.IsChecked = true;
                }
                else
                {
                    each.IsChecked = false;
                }

                each.IsCheckable = true;
            }
            return appth;
        }
    }
}
