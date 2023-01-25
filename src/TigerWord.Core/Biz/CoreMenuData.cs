using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using TigerWord.Core.Services;
using TigerWord.Core.ViewModels;

namespace TigerWord.Core.Biz
{
    public class CoreMenuData : BindableBase
    {
        // EventAggregator
        public static IEventAggregator EA;
        public string Header { get; set; }
        public string ModuleID { get; set; }
        public string CommandID { get; set; }
        public string ToolTip { get; set; }
        public ObservableCollection<CoreMenuData> MenuItems { get; set; }
        //public DelegateCommand<object> Command { get; set; }

        private DelegateCommand<object> _execMenuCmd;
        //public DelegateCommand<string> Command;
        public DelegateCommand<object> Command =>
            _execMenuCmd ?? (_execMenuCmd = new DelegateCommand<object>(ExecMenuCmdFunc));
        void ExecMenuCmdFunc(object param)
        {
            string menu_txt = ModuleID + ":" + CommandID;
            if (CoreMenuData.EA != null)
                CoreMenuData.EA.GetEvent<MenuSentEvent>().Publish(menu_txt);
        }
        //public MenuItemViewModel(string header, DelegateCommand command, string tooltip = "")
        public CoreMenuData()//tring Header, string tooltip = "")
        {
            //Header = header;
            //Command = command;
            //this.ChangeAccentCommand = new SimpleCommand(o => true, this.DoChangeTheme);
        }

        public CoreMenuData(string header, string module_id, string command_id, DelegateCommand<object> command, ObservableCollection<CoreMenuData> menuItmes, string tooltip="")//tring Header, string tooltip = "")
        {
            Header = header;
            if(command != null)
            {
                _execMenuCmd = command;
            }
            ModuleID = module_id;
            CommandID = command_id;
            MenuItems = menuItmes;
            ToolTip = tooltip;
            //this.ChangeAccentCommand = new SimpleCommand(o => true, this.DoChangeTheme);
        }


        public CoreMenuData(string header, string tooltip = "")
        {
            Header = header;
            ToolTip= tooltip;
            //Command = command;
            //this.ChangeAccentCommand = new SimpleCommand(o => true, this.DoChangeTheme);
        }
    }
}
