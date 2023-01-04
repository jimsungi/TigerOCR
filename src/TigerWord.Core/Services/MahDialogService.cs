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
        void ShowMessage(string message);
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

        public async void ShowMessage(string message)
        {
            if (win!=null && message != null)
            {
                // This demo runs on .Net 4.0, but we're using the Microsoft.Bcl.Async package so we have async/await support
                // The package is only used by the demo and not a dependency of the library!
                var mySettings = new MetroDialogSettings()
                {
                    AffirmativeButtonText = "Hi",
                    NegativeButtonText = "Go away!",
                    FirstAuxiliaryButtonText = "Cancel",
                    ColorScheme = win.MetroDialogOptions.ColorScheme,
                    DialogButtonFontSize = 20D
                };

                MessageDialogResult result = await win.ShowMessageAsync("Hello!", "Welcome to the world of metro!",
                                                                         MessageDialogStyle.AffirmativeAndNegativeAndSingleAuxiliary, mySettings);

                if (result != MessageDialogResult.FirstAuxiliary)
                    await win.ShowMessageAsync("Result", "You said: " + (result == MessageDialogResult.Affirmative
                                                    ? mySettings.AffirmativeButtonText
                                                    : mySettings.NegativeButtonText +
                                                      Environment.NewLine + Environment.NewLine + "This dialog will follow the Use Accent setting."));

            }
        }
    }
}
