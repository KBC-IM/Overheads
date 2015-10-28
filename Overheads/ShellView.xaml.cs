using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Overheads.Helpers;
using Overheads.Properties;

namespace Overheads
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

            //WPF does not have a focused Item when it first starts up. This puts focus on the first element.
            //This happens to be the main grid which has the key capture logic. Thus we are able to capture the keys.
            Loaded += (sender, e) =>
                    MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); 

        }

        private void Shell_Loaded(object sender, RoutedEventArgs e)
        {
            if (Settings.Default.UpgradeRequired)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeRequired = false;
                Settings.Default.Save();
            }

            if (Core.Properties.Settings.Default.UpgradeRequired)
            {
                Core.Properties.Settings.Default.Upgrade();
                Core.Properties.Settings.Default.UpgradeRequired = false;
                Core.Properties.Settings.Default.Save();
            }
        }

        private void Shell_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Settings.Default.MainWindowPlacement = this.GetPlacement();
            Settings.Default.Save();
            Core.Properties.Settings.Default.Save();
        }

        private void Shell_SourceInitialized(object sender, System.EventArgs e)
        {
            if (Settings.Default.Fullscreen)
            {
                Console.WriteLine(Settings.Default.MaximizeToSecondary);
                if (Settings.Default.MaximizeToSecondary)
                    WindowExt.MaximizeToSecondary(this);
                else
                    WindowExt.MaximizeToPrimary(this);
            }
            else
                this.SetPlacement(Settings.Default.MainWindowPlacement);
        }

        private void Shell_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void MainGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.RightAlt) && Keyboard.IsKeyDown(Key.Enter))
            {
                WindowExt.ToggleFullscreen(this);
            }
        }
    }
}
