using Prism.Commands;

namespace TigerWord.Core.ViewModels
{
    public interface IApplicationCommands
    {
        CompositeCommand SaveCommand { get; }
        CompositeCommand SkipAddCommand { get; }
        CompositeCommand ThemeMenuReloadCommand { get; }
        CompositeCommand ThemeAccentReloadCommand { get; }
    }

    public class ApplicationCommands : IApplicationCommands
    {
        private CompositeCommand _saveCommand = new CompositeCommand();
        private CompositeCommand _skipAddCommand = new CompositeCommand();
        private CompositeCommand _themeMenuReloadCommand = new CompositeCommand();
        private CompositeCommand _themeAccentReloadCommand = new CompositeCommand();
        public CompositeCommand SaveCommand
        {
            get { return _saveCommand; }
        }
        public CompositeCommand SkipAddCommand
        {
            get { return _skipAddCommand; }
        }

        public CompositeCommand ThemeMenuReloadCommand
        {
            get {
                return _themeMenuReloadCommand; 
            }
        }

        public CompositeCommand ThemeAccentReloadCommand
        {
            get
            {
                return _themeMenuReloadCommand;
            }
        }
    }
}
