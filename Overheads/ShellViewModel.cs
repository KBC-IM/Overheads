using System.Windows;
using Caliburn.Micro;
using Overheads.Core;
using Overheads.ViewModels;
using System.Windows.Input;

namespace Overheads {
    public class ShellViewModel : Conductor<IScreen>, IShell
    {
        public MainViewModel Main { get; set; }
        public EditViewModel Edit { get; set; }

        public ShellViewModel()
        {
            DisplayName = "Overheads";
            Main = new MainViewModel();
            Edit = new EditViewModel();
        }
        protected override void OnActivate()
        {
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
                    {
                        Main.OnKeyPress(e);   
                    }
                    break;
            }
            
        }

        public void GoIntoEditMode()
        {
            if (ActiveItem is MainViewModel)
            {
                if (Main.CurrentSong != null)
                {
                    Edit.CurrentSong = Main.CurrentSong;
                    ActivateItem(Edit);
                }
                else
                {
                    //create a new file and save it
                }
            }
            else
            {
                BookManager.SaveSong(Edit.CurrentSong);
                ActivateItem(Main);
                Main.CurrentSong = BookManager.LoadSong(Edit.CurrentSong.Key);
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
    }
}