using System.Windows.Input;
using Caliburn.Micro;
using Overheads.Core;

namespace Overheads.ViewModels
{
    public class EditViewModel : Screen
    {
        private Song _currentSong;
        public Song CurrentSong
        {
            get { return _currentSong; }
            set
            {
                if (Equals(value, _currentSong)) return;
                _currentSong = value;
                NotifyOfPropertyChange(() => CurrentSong);
            } 
        }
    }
}
