using Caliburn.Micro;
using Overheads.ViewModels;

namespace Overheads {
    public class ShellViewModel : Conductor<IScreen>, IShell
    {
        public MainViewModel Main { get; set; }

        public ShellViewModel()
        {
            Main = new MainViewModel();
        }
        protected override void OnActivate()
        {
            ActivateItem(Main);
            base.OnActivate();
        }
    }
}