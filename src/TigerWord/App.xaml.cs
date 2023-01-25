using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Prism.Ioc;
using Prism.Unity;
using Prism.Services;
using Prism.Modularity;
using TigerWord.Core.ViewModels;
using Prism.Mvvm;
using System.Reflection;
using TigerWord.Views;
using TigerWord.ViewModels;
using Unity;
using TigerWord.Core.Services;

namespace TigerWord
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : PrismApplication
    {
        public App()
        {
            //Startup += (sender, args) =>
            //{
            //    // Type1 : Use WpfApp1.Splash As SplashWindow
            //    //com.tigerword.twsplash.Splash
            //    //.Create()
            //    //.UseSplash<WpfApp1.Splash>()
            //    //.UseWindow<WpfApp1.MainWindow>()
            //    //.Wait(4000)
            //    //.Run();
            //    // @TODO Type2 : Use Spash - Default Splash (with default splash image)  As SplashWindow
            //    //com.tigerword.splash.Splash
            //    //.Create()
            //    //.UseDefaultSplash()
            //    //.UseWindow<TigerOCR.MainWindow>()
            //    //.Wait(4000)
            //    //.Run();
            //    // @TODO Type3 : Use Image (with default splash window) As SplashWindow
            //    //com.tigerword.twsplash.Splash
            //    //.Create()
            //    //.UseImage(new System.Windows.Media.Imaging.BitmapImage())
            //    //.UseWindow<WpfApp1.MainWindow>()
            //    //.Wait(4000)
            //    //.Run();
            //};
        }

        protected override Window CreateShell()
        {
            com.tigerword.splash.Splash
            .Create()
            .UseDefaultSplash()
            //.UseWindow<TigerOCR.MainWindow>()
            .Wait(0) // .Wait(4000)
            .RunSplashOnly();
            var w = Container.Resolve<MainWindow>();
            return w;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // pretend to register a service
            //containerRegistry.Register<Services.ISampleService, Services.DbSampleService>();
            // register other needed services here

            // Register Application Range Command
            containerRegistry.RegisterSingleton<IApplicationCommands, ApplicationCommands>();
            containerRegistry.RegisterSingleton<IMahDialogService, MahDialogService>();
            containerRegistry.RegisterSingleton<IMahThemeService, MahThemeService>();
            containerRegistry.RegisterSingleton<IUserSettingService, UserSettingService>();

        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            var ocrmodule = typeof(TigerWord.GoOCR.Views.TigerOCR);
            moduleCatalog.AddModule(new ModuleInfo()
            {
                ModuleName = ocrmodule.Name,
                ModuleType = ocrmodule.AssemblyQualifiedName,
                InitializationMode = InitializationMode.OnDemand
            });
        }

        /// <summary>
        /// Reset Views/xxxx.xaml to ViewModels/xxxxViewModel relations
        /// </summary>
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            // Reset to Views/xxxx.xaml to CustomNamespace/xxxxViewModel

            //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            //{
            //    string viewName = viewType.FullName;//.Replace(".ViewModels.", ".CustomNamespace.");
            //    switch (viewType)
            //    {
            //        case "":
            //            break;
            //    }
            //    var viewName = viewType.FullName.Replace(".ViewModels.", ".CustomNamespace.");
            //    var viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            //    var viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
            //    return Type.GetType(viewModelName);
            //});
            //ViewModelLocationProvider.SetDefaultViewTypeToViewModelTypeResolver((viewType) =>
            //{
            //    string fullName = viewType.FullName;//.Replace(".ViewModels.", ".CustomNamespace.");
            //    string viewModelName = "";
            //    var viewAssemblyName = "";
            //    switch (fullName)
            //    {
            //        case "TigerWord.Views.Main":
            //            viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            //            viewModelName = $"TigerWord.ViewModels.MainViewModel, {viewAssemblyName}";
            //            break;
            //        case "TigerWord.ViewModels.MainViewModel":
            //            viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            //            viewModelName = $"TigerWord.ViewModels.MainViewModel, {viewAssemblyName}";
            //            break;
            //        default:                        
            //            var viewName = viewType.FullName.Replace(".ViewModels.", ".CustomNamespace.");
            //            viewAssemblyName = viewType.GetTypeInfo().Assembly.FullName;
            //            viewModelName = $"{viewName}ViewModel, {viewAssemblyName}";
            //            break;
            //    }
            //    var t = Type.GetType(viewModelName);
            //    return t;
            //});

            //Cannot Run To ViewModel On the Same View (If you want, make it singleton)
            //ViewModelLocationProvider.Register<TigerWord.Views.AdFlip>(() => Container.Resolve<TigerWord.ViewModels.MainViewModel>());
            //ViewModelLocationProvider.Register<TigerWord.Views.Main>(() => Container.Resolve<TigerWord.ViewModels.MainViewModel>());

        }


    }
}
