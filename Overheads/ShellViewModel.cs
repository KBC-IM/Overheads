using System;
using System.ComponentModel;
using System.Windows;
using Caliburn.Micro;
using Overheads.Core;
using Overheads.Helpers;
using Overheads.ViewModels;
using System.Windows.Input;
using Overheads.Properties;

namespace Overheads {
    public class ShellViewModel : Conductor<IScreen>, IShell
    {
        public MainViewModel Main { get; set; }
        public EditViewModel Edit { get; set; }
        public SettingsViewModel Setting { get; set; }

        public ShellViewModel()
        {
            DisplayName = "Overheads";
            Main = new MainViewModel();
            Edit = new EditViewModel();
            Setting = new SettingsViewModel();

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
        protected override void OnActivate()
        {
	    //This activates the main viewmodel
            ActivateItem(Main);
            base.OnActivate();
        }

        public void PreviewKeyUp(KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    Application.Current.Shutdown();
                    break;
                case Key.OemTilde:
                case Key.Oem8:
                    GoIntoEditMode();
                    e.Handled = true;
                    break;
                default:
                    if (ActiveItem is MainViewModel)
                        Main.OnKeyPress(e);
                    break;
            }
          
            if (Keyboard.IsKeyDown(Key.LeftAlt) && Keyboard.IsKeyDown(Key.S))
                GoIntoSettings();
        }

        public void GoIntoEditMode()
        {
            if (ActiveItem is MainViewModel)
            {
                if (Main.CurrentSong != null && !string.IsNullOrEmpty(Main.CurrentSong.Key))
                {
                    Edit.CurrentSong = Main.CurrentSong;
                    ActivateItem(Edit);
                    Console.WriteLine(Main.CurrentSong.Key);
                }
            }
            else
            {
                System.Windows.MessageBoxButton buttons = MessageBoxButton.YesNoCancel;
                System.Windows.MessageBoxResult dr = MessageBox.Show("Would you like to save changes to this song?", "Save Changes", buttons);

                if (dr == System.Windows.MessageBoxResult.Yes)
                {
                    BookManager.SaveSong(Edit.CurrentSong);
                    ActivateItem(Main);
                    Main.CurrentSong = BookManager.LoadSong(Edit.CurrentSong.Key);
                    HackTheFocus();
                }
                else if (dr == System.Windows.MessageBoxResult.No)
                {
                    ActivateItem(Main);
                    Main.CurrentSong = BookManager.LoadSong(Edit.CurrentSong.Key);
                    HackTheFocus();
                }
            } 
        }

        public void GoIntoSettings()
        {
            if(ActiveItem is MainViewModel)
            {
                ActivateItem(Setting);
            }
            else
            {
                ActivateItem(Main);
                HackTheFocus();
            }
        }

        public void HackTheFocus()
        {
            var view = ((ShellView)this.GetView());
            while (view.IsFocused == false)
            {
                FocusManager.SetIsFocusScope(view, true);
                FocusManager.SetFocusedElement(view, view);
            }

            view.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public static void OnSourceInitialized(object sender, EventArgs e)
        {
            Console.WriteLine("Source Initialized");
            Window window = sender as Window;
            if (Settings.Default.Fullscreen)
            {
                Console.WriteLine(Settings.Default.MaximizeToSecondary);
                if (Settings.Default.MaximizeToSecondary)
                    WindowExt.MaximizeToSecondary(window);
                else
                    WindowExt.MaximizeToPrimary(window);
            }
            else
                window.SetPlacement(Settings.Default.MainWindowPlacement);
        }

        public static void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Console.WriteLine("Closing");
            Window window = sender as Window;
            Settings.Default.MainWindowPlacement = window.GetPlacement();
            Settings.Default.Save();
            Core.Properties.Settings.Default.Save();
        }

        public static void OnKeyDown(object sender, EventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.RightAlt) && Keyboard.IsKeyDown(Key.Enter))
            {
                WindowExt.ToggleFullscreen(sender as Window);
            }
        }
    }
}