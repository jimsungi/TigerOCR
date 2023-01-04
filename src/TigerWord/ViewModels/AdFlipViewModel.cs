using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TigerWord.Core.ViewModels;
using Microsoft.VisualBasic;
using System.Windows.Input;

namespace TigerWord.ViewModels
{
    internal class AdFlipViewModel : BindableBase
    {

        public AdFlipViewModel(IApplicationCommands applicationCommands)
        {
            ApplicationCommands = applicationCommands;
            //UpdateCommand = new DelegateCommand(Update).ObservesCanExecute(() => CanUpdate);

           
        }
        private IApplicationCommands _applicationCommands;
        public IApplicationCommands ApplicationCommands
        {
            get { return _applicationCommands; }
            set { SetProperty(ref _applicationCommands, value); }
        }

    }
}
