using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Prism.Services.Dialogs;
using Prism.Unity;

namespace TigerWord.Core.Services
{
    public interface IMahDialogService
    {
        void SetMahWindow(MetroWindow _win);
        void ShowMessage(string message, string title="");
    }
    public class MahDialogService : IMahDialogService
    {
        MetroWindow? win = null;
        public void SetMahWindow(MetroWindow _win)
        {

            win = _win;
        }
        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            throw new NotImplementedException();
        }

        public void Show(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            throw new NotImplementedException();
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback)
        {
            throw new NotImplementedException();
        }

        public void ShowDialog(string name, IDialogParameters parameters, Action<IDialogResult> callback, string windowName)
        {
            throw new NotImplementedException();
        }

        public async void ShowMessage(string message, string title="")
        {
            if (win!=null && message != null)
            {
                // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
                // The package is only used by the demo and not a dependency of the library!
                var mySettings = new MetroDialogSettings()
                {
                    DefaultText= message,
                    AffirmativeButtonText = "OK",
                    ColorScheme = win.MetroDialogOptions.ColorScheme,
                    DialogButtonFontSize = 20D
                };

                MessageDialogResult result = await win.ShowMessageAsync(title, message,
                                                                         MessageDialogStyle.Affirmative, mySettings);                
            }           
        }
    }
}
