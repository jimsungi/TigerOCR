using Prism.Mvvm;
using System.Collections.Generic;
using Unity;
using Prism.Commands;
using Microsoft.Win32;
using Prism.Events;
using TigerWord.Core.Biz;
using TigerWord.Core.Services;
using TigerWord.Core.ViewModels;
using System.Linq;
using System.Reflection;

namespace TigerWord.GoOCR.ViewModels
{
    public class TigerOCRViewModel : BindableBase
    {
        public TigerOCRViewModel(IUserSettingService _UserSettingService, IEventAggregator ea)
        {
            // Menu Event Aggregator
            _ea = ea;
            _ea.GetEvent<MenuSentEvent>().Subscribe(MenuExecuted);
            RunMenuCommand = new DelegateCommand<object>(RunMenu);
            // Service
        }
        #region Property
        private int _pageIndex=1;
        public int PageIndex
        {
            get
            {
                return _pageIndex;
            }
            set
            {
                SetProperty(ref _pageIndex, value);
            }
        }

        #endregion

        #region MenuReceive , MenuSend
        private void MenuExecuted(object menu)
        {
            string menu_txt = menu as string;
            string[] menu_path = menu_txt.Split(":");
            if (menu_path != null && menu_path.Length == 2)
            {
                if (menu_path[0] == GoOCR.ModuleID)
                {
                    switch (menu_path[1])
                    {
                        case "stepview":
                            PageIndex = 0;
                            break;
                        case "standardview":
                            PageIndex = 1;
                            break;
                        //    ExitFunc();
                        //    break;
                        //case "SETTING":
                        //    SettingFunc();
                            //break;
                        default:
                            break;
                    }
                }
            }            // Read type of menu // 
        }
        public DelegateCommand<object> RunMenuCommand { get; set; }

        private void RunMenu(object param)
        {
            _ea.GetEvent<MenuSentEvent>().Publish(param);
        }

        #endregion

        #region Services
        private IUserSettingService? _userSettingService;
        private IEventAggregator _ea;
        public IUserSettingService? UserSettingService
        {
            get => _userSettingService;
            set => SetProperty(ref _userSettingService, value);
        }
        #endregion Services

    }
}
