using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using OpenCvSharp.Flann;
using Prism.Commands;
using Prism.Mvvm;
using TigerWord.Core.Services;
using TigerWord.Core.ViewModels;

namespace TigerWord.ViewModels
{
    public class MainViewModel : BindableBase
    {
        
        public MainViewModel()
        {
        }


        public MainViewModel(IApplicationCommands applicationCommands,IMahDialogService dialogService)
        {
            ApplicationCommands = applicationCommands;
            MahDialogService = dialogService;
            ChangeStartPageTabCmd = new DelegateCommand<string>(ChangeStartPageTabFunc).ObservesCanExecute(() => ChangeStartPageTabCmdEnabled);
            _applicationCommands.SaveCommand.RegisterCommand(ChangeStartPageTabCmd);
        }

        #region Property

        /// <summary>
        /// 
        /// </summary>
        private string _updateText;
        public string UpdateText
        {
            get { return _updateText; }
            set
            {
                SetProperty(ref _updateText, value);
            }
        }

        private int _mainTabIndex = 0;
        public int MainTabIndex
        {
            get
            {
                return _mainTabIndex;
            }
            set
            {
                SetProperty(ref _mainTabIndex, value);
            }
        }
        #endregion

        #region Application Command
        /// <summary>
        /// Application Commands
        /// </summary>
        private IApplicationCommands _applicationCommands;
        private IMahDialogService dialogService;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }

        public IMahDialogService MahDialogService
        {
            get { return dialogService; }
            set { SetProperty(ref dialogService, value); }
        }

        #region ChangeStartPageTabCmd
        public DelegateCommand<string> ChangeStartPageTabCmd { get; private set; }
        // Default true(enabled)
        private bool _ChangeStartPageTabEnabled=true;
        public bool ChangeStartPageTabCmdEnabled
        {
            get { return _ChangeStartPageTabEnabled; }
            set
            {
                SetProperty(ref _ChangeStartPageTabEnabled, value);
                ChangeStartPageTabCmd.RaiseCanExecuteChanged();
            }
        }
        private void ChangeStartPageTabFunc(string parameter)
        {            
            switch (parameter)
            {
                case "Ad":
                    MainTabIndex = 0;
                    break;
                case "Main":
                default:
                    MainTabIndex = 1;
                    break;
            }
        }

        #endregion
        #endregion




   
    }
}
