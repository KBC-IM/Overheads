using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Overheads.Core
{
    public class Book : INotifyPropertyChanged
    {
        private string _title;
        public string Key { get; set; }
        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("Title");
            }
        }

        public List<SearchSong> Songs { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
